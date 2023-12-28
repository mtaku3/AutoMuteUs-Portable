use abi_stable::{std_types::{RResult, RBoxError}, sabi_extern_fn};
use amup_core_config_intf::Config;

#[abi_stable::sabi_trait]
pub trait Component {
    fn start(&mut self, config: &Config) -> RResult<(), RBoxError>;
}
