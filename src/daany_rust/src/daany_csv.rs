use std::ffi::{CString, CStr};
use std::fs::File;
use std::io::{Write, BufWriter,BufRead, BufReader};
use std::os::raw::{c_char, c_int, c_float, c_double};
use chrono::{DateTime,NaiveDateTime, TimeZone};
use chrono::Utc;


#[repr(C)]
pub union CellValue {
    pub int_value: c_int,
    pub long_value: i64,
    pub float_value: c_float,
    pub double_value: c_double,
    pub string_value: *const c_char,
    pub datetime_value: i64,  // Ensure DateTime is stored as i64 (milliseconds since epoch)
}

#[repr(C)]
pub struct CellObject {
    pub value: CellValue,
    pub type_id: c_int,
}


#[no_mangle]
pub extern "C" fn from_csv(
    file_path: *const c_char,
    separator: c_char,
    date_format: *const c_char,
    missing_value: *const c_char,
    has_header: bool,
    columns_out: *mut *mut *const c_char, 
    col_count_out: *mut usize,
    data_out: *mut *mut CellObject, 
    row_count_out: *mut usize
) {
    // Parse input strings
    let file_path_str = unsafe { CStr::from_ptr(file_path).to_str().expect("Invalid file path") };
    let date_format_str = unsafe { CStr::from_ptr(date_format).to_str().unwrap_or("%Y-%m-%d") };
    let _missing_str = unsafe { CStr::from_ptr(missing_value).to_str().unwrap_or("[Missing]") };

    let sep = separator as u8 as char;
    let file = File::open(file_path_str).expect("Failed to open CSV file");
    let reader = BufReader::new(file);

    let mut lines = reader.lines();
    let mut columns: Vec<String> = Vec::new();
    let mut rows: Vec<Vec<CellObject>> = Vec::new();

    // Read header if present
    if has_header {
        if let Some(Ok(header_line)) = lines.next() {
            columns = header_line.split(sep).map(|s| s.to_string()).collect();
        }
    }

    // Process data rows
    for line in lines {
        if let Ok(row) = line {
            let cells: Vec<CellObject> = row.split(sep).enumerate().map(|(_i, cell_str)| {
                if cell_str.is_empty() || cell_str == _missing_str {
                    return CellObject { 
                        value: CellValue { string_value: std::ptr::null() }, 
                        type_id: 4 
                    };
                }

                // Only attempt to parse as a date if date_format is explicitly provided
                if !date_format_str.is_empty() {
                    if let Ok(parsed_dt) = NaiveDateTime::parse_from_str(cell_str, date_format_str) {
                        let timestamp = Utc.from_utc_datetime(&parsed_dt).timestamp_millis();
                        return CellObject { value: CellValue { datetime_value: timestamp }, type_id: 5 };
                    }
                }

                // Type inference for common types
                if let Ok(n) = cell_str.parse::<i32>() {
                    return CellObject { value: CellValue { int_value: n }, type_id: 0 };
                } else if let Ok(n) = cell_str.parse::<i64>() {
                    return CellObject { value: CellValue { long_value: n}, type_id: 1 };
                } else if let Ok(n) = cell_str.parse::<f32>() {
                    return CellObject { value: CellValue { float_value: n }, type_id: 2 };
                } else if let Ok(n) = cell_str.parse::<f64>() {
                    return CellObject { value: CellValue { double_value: n }, type_id: 3 };
                } else {
                    // Allocate string value dynamically
                    let c_string = CString::new(cell_str).expect("CString allocation failed");
                    let c_ptr = c_string.into_raw();
                    
                    return CellObject { value: CellValue { string_value: c_ptr }, type_id: 4 };
                }
            }).collect();

            rows.push(cells);
        }
    }


    // Convert to raw pointers for FFI output
    unsafe {

        *col_count_out = columns.len();
        *row_count_out = rows.len(); 
        
        let mut cols_vec: Vec<*const c_char> = columns.iter()
            .map(|s| CString::new(s.as_str()).unwrap().into_raw() as *const c_char) // Ensure `*const c_char`
            .collect();
        // Use heap allocation (Box) instead of `.as_mut_ptr()`
        *columns_out = Box::into_raw(cols_vec.into_boxed_slice()) as *mut *const c_char;

        // Convert cell data, ensuring string pointers remain valid after moving the data
        let mut cloned_data_vec: Vec<CellObject> = rows.into_iter().flatten().map(|mut cell| {
            if cell.type_id == 4 && !cell.value.string_value.is_null() {

                // Deep clone the string to ensure a valid pointer after moving the struct
                let original_str = unsafe { CStr::from_ptr(cell.value.string_value).to_string_lossy().into_owned() };
                let cloned_cstring = CString::new(original_str).expect("CString allocation failed");
                cell.value.string_value = cloned_cstring.into_raw();
            }
            cell
        }).collect();

        // Allocate memory in heap for safe FFI interaction
        let boxed_data = cloned_data_vec.into_boxed_slice();
        *data_out = Box::into_raw(boxed_data) as *mut CellObject;
        
    }
}


/// Converts a CellObject into a formatted Rust string for CSV
/// Formats a cell value correctly for CSV output
fn format_csv_value(cell: &CellObject, separator: char, date_format: &str) -> String {
    unsafe {
        match cell.type_id {
            0 => format!("{}", cell.value.int_value),
            1 => format!("{}", cell.value.long_value),
            2 => format!("{}", cell.value.float_value),
            3 => format!("{}", cell.value.double_value),
            4 => {
                let c_str = CStr::from_ptr(cell.value.string_value);
                let raw_str = c_str.to_str().unwrap_or("[Invalid UTF-8]");
                // Quote only if it contains the separator
                if raw_str.contains(separator) {
                    format!("\"{}\"", raw_str)
                } else {
                    raw_str.to_string()
                }
            }
            5 => {
                if let Some(datetime) = DateTime::from_timestamp_millis(cell.value.datetime_value) {
                    datetime.format(date_format).to_string()
                } else {
                    "[Invalid DateTime]".to_string()
                }
            }
            _ => "Unknown".to_string(),
        }
    }
}

/// Rust function to export data to CSV at a specified file path
/// Saves data into a properly formatted CSV file
#[no_mangle]
pub extern "C" fn to_csv(
    file_path: *const c_char,
    data: *const CellObject, 
    data_length: usize, 
    columns: *const *const c_char, 
    col_length: usize, 
    separator: c_char, 
    has_header: bool, 
    date_format: *const c_char
) {
    // Parse file path
    let file_path_str = unsafe {
        CStr::from_ptr(file_path).to_str().expect("Invalid file path")
    };

    // Open CSV file for writing
    let file = File::create(file_path_str).expect("Failed to create file");
    let mut writer = BufWriter::new(file);

    // Parse separator
    let sep = separator as u8 as char;

    // Parse date format
    let date_format_str = unsafe {
        CStr::from_ptr(date_format).to_str().unwrap_or("%Y-%m-%d")
    };

    // Automatically calculate row count
    let row_count = data_length / col_length;

    unsafe {
        // Write headers if required
        if has_header {
            let header: Vec<String> = (0..col_length)
                .map(|i| {
                    let col_str = CStr::from_ptr(*columns.add(i));
                    col_str.to_str().unwrap_or("[Invalid Column]").to_string()
                })
                .collect();
            writeln!(writer, "{}", header.join(&sep.to_string())).expect("Failed to write header");
        }

        // Write each row separately
        for row in 0..row_count {
            let row_values: Vec<String> = (0..col_length)
                .map(|col| {
                    let cell = &*data.add(row * col_length + col);
                    format_csv_value(cell, sep, date_format_str)
                })
                .collect();

            writeln!(writer, "{}", row_values.join(&sep.to_string())).expect("Failed to write row");
        }
    }

    writer.flush().expect("Failed to flush CSV writer");
}


#[no_mangle]
pub extern "C" fn free_columns(columns: *mut *const c_char, col_count: usize) {
    unsafe {
        if columns.is_null() {
            return; // Prevent null pointer access
        }

        let slice = std::slice::from_raw_parts(columns, col_count);
        for &col_ptr in slice.iter() {
            if !col_ptr.is_null() {
                let _ = CString::from_raw(col_ptr as *mut c_char); // Free individual strings
            }
        }

        //  Only free the array if it was allocated using Box::into_raw()
        drop(Box::from_raw(columns));
    }
}





#[no_mangle]
pub extern "C" fn free_data(data: *mut CellObject, row_count: usize, col_count: usize) {
    unsafe {
        if data.is_null() {
            return;
        }

        let slice = std::slice::from_raw_parts_mut(data, row_count * col_count);

        for cell in slice.iter_mut() {
            if cell.type_id == 4 && !cell.value.string_value.is_null() {
                let _ = CString::from_raw(cell.value.string_value as *mut c_char); // Free allocated string values
            }
        }

        drop(Box::from_raw(data)); // Free the allocated memory for CellObject
    }
}
