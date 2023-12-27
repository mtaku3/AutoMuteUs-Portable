use abi_stable::{
    declare_root_module_statics, library::RootModule, package_version_strings,
    sabi_types::VersionStrings, std_types::RBox, StableAbi,
};
use component::Component_TO;

pub mod component;

#[repr(C)]
#[derive(StableAbi)]
#[sabi(kind(Prefix))]
pub struct ComponentMod {
    pub new: extern "C" fn() -> Component_TO<'static, RBox<()>>,
}

impl RootModule for ComponentMod_Ref {
    declare_root_module_statics! {ComponentMod_Ref}

    const BASE_NAME: &'static str = "component";
    const NAME: &'static str = "component";
    const VERSION_STRINGS: VersionStrings = package_version_strings!();
}
