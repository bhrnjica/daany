use std::ffi::c_char;

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

/// Min calculation for Series data with different numeric types
/// 
/// # Arguments
/// * `data` - Pointer to array of CellObject values
/// * `length` - Number of elements in the array
/// 
/// # Returns
/// Minimum as f64, or NaN if no valid numeric values found
#[no_mangle]
pub extern "C" fn series_min(data: *const CellObject, length: usize) -> f64 {
    if data.is_null() || length == 0 {
        return f64::NAN;
    }

    let data_slice = unsafe { std::slice::from_raw_parts(data, length) };
    let mut min_value = f64::INFINITY;
    let mut has_numeric_values = false;

    for cell in data_slice {
        match cell.type_id {
            1 => { // I32 - int
                let value = unsafe { cell.value.int_value } as f64;
                min_value = min_value.min(value);
                has_numeric_values = true;
            },
            2 => { // I64 - long  
                let value = unsafe { cell.value.long_value } as f64;
                min_value = min_value.min(value);
                has_numeric_values = true;
            },
            3 => { // F32 - float
                let value = unsafe { cell.value.float_value } as f64;
                if !value.is_nan() {
                    min_value = min_value.min(value);
                    has_numeric_values = true;
                }
            },
            5 => { // DD - double
                let value = unsafe { cell.value.double_value };
                if !value.is_nan() {
                    min_value = min_value.min(value);
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
        min_value
    } else {
        f64::NAN
    }
}

/// Max calculation for Series data with different numeric types
/// 
/// # Arguments
/// * `data` - Pointer to array of CellObject values
/// * `length` - Number of elements in the array
/// 
/// # Returns
/// Maximum as f64, or NaN if no valid numeric values found
#[no_mangle]
pub extern "C" fn series_max(data: *const CellObject, length: usize) -> f64 {
    if data.is_null() || length == 0 {
        return f64::NAN;
    }

    let data_slice = unsafe { std::slice::from_raw_parts(data, length) };
    let mut max_value = f64::NEG_INFINITY;
    let mut has_numeric_values = false;

    for cell in data_slice {
        match cell.type_id {
            1 => { // I32 - int
                let value = unsafe { cell.value.int_value } as f64;
                max_value = max_value.max(value);
                has_numeric_values = true;
            },
            2 => { // I64 - long  
                let value = unsafe { cell.value.long_value } as f64;
                max_value = max_value.max(value);
                has_numeric_values = true;
            },
            3 => { // F32 - float
                let value = unsafe { cell.value.float_value } as f64;
                if !value.is_nan() {
                    max_value = max_value.max(value);
                    has_numeric_values = true;
                }
            },
            5 => { // DD - double
                let value = unsafe { cell.value.double_value };
                if !value.is_nan() {
                    max_value = max_value.max(value);
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
        max_value
    } else {
        f64::NAN
    }
}

/// Standard deviation calculation for Series data with different numeric types
/// 
/// # Arguments
/// * `data` - Pointer to array of CellObject values
/// * `length` - Number of elements in the array
/// 
/// # Returns
/// Standard deviation as f64, or NaN if no valid numeric values found
#[no_mangle]
pub extern "C" fn series_std(data: *const CellObject, length: usize) -> f64 {
    if data.is_null() || length == 0 {
        return f64::NAN;
    }

    let data_slice = unsafe { std::slice::from_raw_parts(data, length) };
    let mut values = Vec::new();

    // First pass: collect numeric values
    for cell in data_slice {
        match cell.type_id {
            1 => { // I32 - int
                let value = unsafe { cell.value.int_value } as f64;
                values.push(value);
            },
            2 => { // I64 - long  
                let value = unsafe { cell.value.long_value } as f64;
                values.push(value);
            },
            3 => { // F32 - float
                let value = unsafe { cell.value.float_value } as f64;
                if !value.is_nan() {
                    values.push(value);
                }
            },
            5 => { // DD - double
                let value = unsafe { cell.value.double_value };
                if !value.is_nan() {
                    values.push(value);
                }
            },
            _ => {
                // Skip non-numeric types (strings, dates, booleans, etc.)
                continue;
            }
        }
    }

    if values.len() < 2 {
        return f64::NAN;
    }

    // Calculate mean
    let sum: f64 = values.iter().sum();
    let mean = sum / values.len() as f64;

    // Calculate variance
    let variance: f64 = values.iter()
        .map(|&x| (x - mean).powi(2))
        .sum::<f64>() / (values.len() - 1) as f64; // Sample standard deviation (n-1)

    variance.sqrt()
}

/// Median calculation for Series data with different numeric types
/// 
/// # Arguments
/// * `data` - Pointer to array of CellObject values
/// * `length` - Number of elements in the array
/// 
/// # Returns
/// Median as f64, or NaN if no valid numeric values found
#[no_mangle]
pub extern "C" fn series_median(data: *const CellObject, length: usize) -> f64 {
    if data.is_null() || length == 0 {
        return f64::NAN;
    }

    let data_slice = unsafe { std::slice::from_raw_parts(data, length) };
    let mut values = Vec::new();

    // Collect numeric values
    for cell in data_slice {
        match cell.type_id {
            1 => { // I32 - int
                let value = unsafe { cell.value.int_value } as f64;
                values.push(value);
            },
            2 => { // I64 - long  
                let value = unsafe { cell.value.long_value } as f64;
                values.push(value);
            },
            3 => { // F32 - float
                let value = unsafe { cell.value.float_value } as f64;
                if !value.is_nan() {
                    values.push(value);
                }
            },
            5 => { // DD - double
                let value = unsafe { cell.value.double_value };
                if !value.is_nan() {
                    values.push(value);
                }
            },
            _ => {
                // Skip non-numeric types (strings, dates, booleans, etc.)
                continue;
            }
        }
    }

    if values.is_empty() {
        return f64::NAN;
    }

    // Sort values
    values.sort_by(|a, b| a.partial_cmp(b).unwrap_or(std::cmp::Ordering::Equal));
    
    let len = values.len();
    let middle_index = len / 2;

    if len % 2 == 1 {
        // Odd number of elements
        values[middle_index]
    } else {
        // Even number of elements - take average of two middle values
        (values[middle_index - 1] + values[middle_index]) / 2.0
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::ptr;

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

    #[test]
    fn test_series_min_integers() {
        let data = vec![
            CellObject { value: CellValue { int_value: 5 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 2 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 8 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 1 }, type_id: 1 },
        ];

        let result = series_min(data.as_ptr(), data.len());
        assert_eq!(result, 1.0);
    }

    #[test]
    fn test_series_max_mixed_types() {
        let data = vec![
            CellObject { value: CellValue { int_value: 5 }, type_id: 1 },
            CellObject { value: CellValue { double_value: 2.5 }, type_id: 5 },
            CellObject { value: CellValue { float_value: 8.7 }, type_id: 3 },
            CellObject { value: CellValue { long_value: 10 }, type_id: 2 },
        ];

        let result = series_max(data.as_ptr(), data.len());
        assert_eq!(result, 10.0);
    }

    #[test]
    fn test_series_std_integers() {
        let data = vec![
            CellObject { value: CellValue { int_value: 2 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 4 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 4 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 4 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 5 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 5 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 7 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 9 }, type_id: 1 },
        ];

        let result = series_std(data.as_ptr(), data.len());
        // Expected sample standard deviation for [2, 4, 4, 4, 5, 5, 7, 9] is approximately 2.138
        assert!((result - 2.138).abs() < 0.01);
    }

    #[test]
    fn test_series_median_odd() {
        let data = vec![
            CellObject { value: CellValue { int_value: 1 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 3 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 5 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 7 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 9 }, type_id: 1 },
        ];

        let result = series_median(data.as_ptr(), data.len());
        assert_eq!(result, 5.0);
    }

    #[test]
    fn test_series_median_even() {
        let data = vec![
            CellObject { value: CellValue { int_value: 1 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 2 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 3 }, type_id: 1 },
            CellObject { value: CellValue { int_value: 4 }, type_id: 1 },
        ];

        let result = series_median(data.as_ptr(), data.len());
        assert_eq!(result, 2.5);
    }

    #[test]
    fn test_series_empty_collections() {
        assert!(series_min(ptr::null(), 0).is_nan());
        assert!(series_max(ptr::null(), 0).is_nan());
        assert!(series_std(ptr::null(), 0).is_nan());
        assert!(series_median(ptr::null(), 0).is_nan());
    }
}