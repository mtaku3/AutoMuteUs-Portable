use serde::{Serialize, Deserialize};

#[derive(Debug, Serialize, Deserialize)]
pub enum Command {
    ComponentGetStateRequest(ComponentGetStateRequest),
    ComponentGetStateResponse(ComponentGetStateResponse),
}

#[derive(Debug, Serialize, Deserialize)]
pub enum Component {
    Automuteus,
}


#[derive(Debug, Serialize, Deserialize)]
pub struct ComponentGetStateRequest {
    pub component: Component,
}
#[derive(Debug, Serialize, Deserialize)]
pub struct ComponentGetStateResponse {
    pub component: Component,
}

