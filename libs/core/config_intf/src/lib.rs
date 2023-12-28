use abi_stable::{std_types::RString, StableAbi};
use serde::{Deserialize, Serialize};

#[repr(C)]
#[cfg_attr(feature = "users", derive(Serialize, Deserialize))]
#[derive(Debug, StableAbi)]
pub struct Config {
    pub discord_bot_token: RString,
    pub components: ComponentsKeys,
}

#[repr(C)]
#[cfg_attr(feature = "users", derive(Serialize, Deserialize))]
#[derive(Debug, StableAbi)]
pub struct ComponentsKeys {
    pub automuteus: AutomuteusKeys,
}

#[repr(C)]
#[cfg_attr(feature = "users", derive(Serialize, Deserialize))]
#[derive(Debug, StableAbi)]
pub struct AutomuteusKeys {
    pub port: usize,
}
