use std::panic;

use abi_stable::{
    export_root_module, extern_fn_panic_handling,
    prefix_type::PrefixTypeTrait,
    sabi_extern_fn,
    sabi_trait::TD_Opaque,
    std_types::{
        RBox, RBoxError,
        RResult::{self, ROk},
    },
};
use amup_core_comp_mod_intf::{
    component::{Component, Component_TO},
    ComponentMod, ComponentMod_Ref,
};
use amup_core_config_intf::Config;

#[derive(Debug)]
struct Automuteus {}

impl Component for Automuteus {
    fn start(&mut self, config: &Config) -> RResult<(), RBoxError> {
        extern_fn_panic_handling! {
            no_early_return;

            panic!("Panic test");

            println!("{:?}", config);
            ROk(())
        }
    }
}

#[export_root_module]
fn instantiate_root_module() -> ComponentMod_Ref {
    ComponentMod { new }.leak_into_prefix()
}

#[sabi_extern_fn]
pub fn new() -> Component_TO<'static, RBox<()>> {
    let automuteus = Automuteus {};
    Component_TO::from_value(automuteus, TD_Opaque)
}
