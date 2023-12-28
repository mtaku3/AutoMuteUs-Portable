use abi_stable::std_types::RResult::RErr;
use abi_stable::{library::RootModule, std_types::RResult::ROk};
use amup_core_comp_mod_intf::ComponentMod_Ref;

fn main() {
    let mods = ComponentMod_Ref::load_from_file(
        "../component_modules/automuteus/target/debug/libautomuteus.so".as_ref(),
    )
    .unwrap_or_else(|e| panic!("{}", e));
    let mut comp = mods.new()();
    let config = toml::from_str(
        r#"
        discord_bot_token = "token"

        [components.automuteus]
        port = 5000
    "#,
    )
    .unwrap();
    match comp.start(&config) {
        ROk(_) => (),
        RErr(_) => (),
    };
    println!("Success!");
}
