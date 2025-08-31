use std::ffi::c_char;
use std::ptr;

/// CellObject structure matching the C# equivalent for FFI
#[repr(C)]
#[derive(Clone)]
pub struct CellObject {
    pub value: CellValue,
    pub type_id: i32,
}

/// CellValue union matching the C# equivalent for FFI
#[repr(C)]
#[derive(Clone, Copy)]
pub union CellValue {
    pub int_value: i32,
    pub long_value: i64,
    pub float_value: f32,
    pub double_value: f64,
    pub string_value: *const c_char,
    pub datetime_value: i64,
}

/// Sum calculation for Series data with different numeric types
/// 
/// # Arguments
/// * `data` - Pointer to array of CellObject values
/// * `length` - Number of elements in the array
/// 
/// # Returns
/// Sum as f64, or NaN if no valid numeric values found
#[no_mangle]
pub extern "C" fn series_sum(data: *const CellObject, length: usize) -> f64 {
    if data.is_null() || length == 0 {
        return f64::NAN;
    }

    let data_slice = unsafe { std::slice::from_raw_parts(data, length) };
    let mut sum = 0.0;
    let mut has_numeric_values = false;

    for cell in data_slice {
        match cell.type_id {
            1 => { // I32 - int
                let value = unsafe { cell.value.int_value } as f64;
                sum += value;
                has_numeric_values = true;
            },
            2 => { // I64 - long  
                let value = unsafe { cell.value.long_value } as f64;
                sum += value;
                has_numeric_values = true;
            },
            3 => { // F32 - float
                let value = unsafe { cell.value.float_value } as f64;
                if !value.is_nan() {
                    sum += value;
                    has_numeric_values = true;
                }
            },
            5 => { // DD - double
                let value = unsafe { cell.value.double_value };
                if !value.is_nan() {
                    sum += value;
                    has_numeric_values = true;
                }
            },
            _ => {
                // Skip non-numeric types (strings, dates, booleans, etc.)
                continue;
            }
        }
    }

    if has_numeric_values {
        sum
    } else {
        f64::NAN
    }
}

/// Mean calculation for Series data with different numeric types
/// 
/// # Arguments
/// * `data` - Pointer to array of CellObject values
/// * `length` - Number of elements in the array
/// 
/// # Returns
/// Mean as f64, or NaN if no valid numeric values found
#[no_mangle]
pub extern "C" fn series_mean(data: *const CellObject, length: usize) -> f64 {
    if data.is_null() || length == 0 {
        return f64::NAN;
    }

    let data_slice = unsafe { std::slice::from_raw_parts(data, length) };
    let mut sum = 0.0;
    let mut count = 0;

    for cell in data_slice {
        match cell.type_id {
            1 => { // I32 - int
                let value = unsafe { cell.value.int_value } as f64;
                sum += value;
                count += 1;
            },
            2 => { // I64 - long  
                let value = unsafe { cell.value.long_value } as f64;
                sum += value;
                count += 1;
            },
            3 => { // F32 - float
                let value = unsafe { cell.value.float_value } as f64;
                if !value.is_nan() {
                    sum += value;
                    count += 1;
                }
            },
            5 => { // DD - double
                let value = unsafe { cell.value.double_value };
                if !value.is_nan() {
                    sum += value;
                    count += 1;
                }
            },
            _ => {
                // Skip non-numeric types (strings, dates, booleans, etc.)
                continue;
            }
        }
    }

    if count > 0 {
        sum / count as f64
    } else {
        f64::NAN
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_series_sum_integers() {
        let data = vec![
            CellObject { value: CellValue { int_value: 1 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 2 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 3 }, type_id: 1 },
        ];

        let result = series_sum(data.as_ptr(), data.len());
        assert_eq!(result, 6.0);
    }

    #[test]
    fn test_series_sum_mixed_types() {
        let data = vec![
            CellObject { value: CellValue { int_value: 1 }, type_id: 1 },
            CellObject { value: CellValue { double_value: 2.5 }, type_id: 5 },
            CellObject { value: CellValue { float_value: 1.5 }, type_id: 3 },
        ];

        let result = series_sum(data.as_ptr(), data.len());
        assert_eq!(result, 5.0);
    }

    #[test]
    fn test_series_mean_integers() {
        let data = vec![
            CellObject { value: CellValue { int_value: 2 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 4 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 6 }, type_id: 1 },
        ];

        let result = series_mean(data.as_ptr(), data.len());
        assert_eq!(result, 4.0);
    }

    #[test]
    fn test_series_sum_empty() {
        let result = series_sum(ptr::null(), 0);
        assert!(result.is_nan());
    }
}