
use std::fs::remove_file;

use std::fs::File;
use std::io::{BufRead, BufReader};
use std::ffi::{CString, CStr};
use std::os::raw::c_char;
use chrono::{NaiveDateTime, TimeZone, Utc};



//include load csv 
mod daany_csv; // Import the module
use daany_csv::{to_csv, CellObject, CellValue}; // Import the functions from the module

// Utility function: Check if the file exists and delete it
fn ensure_fresh_csv(file_path: &str) {
    if std::path::Path::new(file_path).exists() {
        remove_file(file_path).expect("Failed to delete existing file");
        println!("Deleted existing file: {}", file_path);
    }
}


// Example function to create sample data for testing
fn generate_sample_data() -> Vec<CellObject> {
    vec![
        CellObject {
            value: CellValue { int_value: 1 },
            type_id: 0,
        },
        CellObject {
            value: CellValue { float_value: 2.3 },
            type_id: 2,
        },
        CellObject {
            value: CellValue { datetime_value: 1714070400000 }, // Unix timestamp for "2024-04-25"
            type_id: 5,
        },
        CellObject {
            value: CellValue {
                string_value: CString::new("Description of the column").unwrap().into_raw(),
            },
            type_id: 4,
        },

        //row 2

        CellObject {
            value: CellValue { int_value: 2 },
            type_id: 0,
        },
        CellObject {
            value: CellValue { float_value: 5.3 },
            type_id: 2,
        },
        CellObject {
            value: CellValue { datetime_value: 1714089600 }, // Unix timestamp for "2024-04-25"
            type_id: 5,
        },
        CellObject {
            value: CellValue {
                string_value: CString::new("*").unwrap().into_raw(), // Correctly represents a null pointer
            },
            type_id: 4,
        },
    ]
}

// Example function to generate column names
fn generate_column_headers() -> Vec<*const c_char> {
    vec![
        CString::new("col0").unwrap().into_raw(),
        CString::new("col1").unwrap().into_raw(),
        CString::new("col2").unwrap().into_raw(),
        CString::new("col3").unwrap().into_raw(),
    ]
}

fn main() {

    let file_path = "output.csv";
    ensure_fresh_csv(file_path);

    let file_path = CString::new(file_path).unwrap().into_raw();
    let data = generate_sample_data();
    let columns = generate_column_headers();
    
    to_csv(
        file_path,
        data.as_ptr(),
        data.len(),
        columns.as_ptr(),
        columns.len(),
        b',' as c_char,
        true,
        CString::new("%Y-%m-%d").unwrap().into_raw(),
    );

    println!("CSV file has been created successfully!");
}
