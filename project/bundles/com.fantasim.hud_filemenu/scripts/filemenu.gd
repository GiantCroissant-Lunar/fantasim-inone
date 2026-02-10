extends Node


func _ready():
	var bootstrap = get_node("/root/Bootstrap")

	# File menu
	var file_menu: PopupMenu = bootstrap.call("get_or_add_menu", "File")
	file_menu.add_item("Load PCK...", 0)
	file_menu.add_item("Unload Bundle", 1)
	file_menu.add_item("Reload Bundle", 2)
	file_menu.add_separator()
	file_menu.add_item("Quit", 3)
	file_menu.id_pressed.connect(_on_file_menu)

	# View menu
	var view_menu: PopupMenu = bootstrap.call("get_or_add_menu", "View")
	view_menu.add_check_item("Inspector", 0)
	view_menu.add_check_item("Timeline", 1)
	view_menu.set_item_checked(0, true)
	view_menu.set_item_checked(1, true)
	view_menu.id_pressed.connect(_on_view_menu)

	# Help menu
	var help_menu: PopupMenu = bootstrap.call("get_or_add_menu", "Help")
	help_menu.add_item("About Fantasim", 0)
	help_menu.id_pressed.connect(_on_help_menu)

	print("[FileMenu Bundle] menus registered")


func _on_file_menu(id: int):
	var bootstrap = get_node("/root/Bootstrap")
	match id:
		0:
			bootstrap.call(
				"show_file_dialog", Callable(self, "_on_file_selected")
			)
		1:
			pass  # TODO: unload selected bundle
		2:
			pass  # TODO: reload selected bundle
		3:
			get_tree().quit()


func _on_file_selected(path: String):
	var bootstrap = get_node("/root/Bootstrap")
	bootstrap.call("load_bundle", path)


func _on_view_menu(id: int):
	pass  # TODO: toggle panel visibility


func _on_help_menu(id: int):
	match id:
		0:
			print("[FileMenu Bundle] Fantasim — Editor-style HUD")
