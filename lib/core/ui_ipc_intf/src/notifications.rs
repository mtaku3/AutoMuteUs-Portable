use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub enum Notifications {
    StdoutNotification(StdoutNotification),
    StderrNotification(StderrNotification),
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StdoutNotification {
    pub message: String,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StderrNotification {
    pub message: String,
}
