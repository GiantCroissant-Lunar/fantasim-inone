extends HBoxContainer


func _ready():
	# Connect button signals via BootstrapShim methods
	$loadPck.pressed.connect(_on_load_pressed)
	$unload.pressed.connect(_on_unload_pressed)
	$reload.pressed.connect(_on_reload_pressed)
	$snapshot.pressed.connect(_on_snapshot_pressed)

	print("[Toolbar Bundle] ready")


func _on_load_pressed():
	var bootstrap = get_node("/root/Bootstrap")
	bootstrap.call(
		"show_file_dialog", Callable(self, "_on_file_selected")
	)


func _on_file_selected(path: String):
	var bootstrap = get_node("/root/Bootstrap")
	bootstrap.call("load_bundle", path)


func _on_unload_pressed():
	var bootstrap = get_node("/root/Bootstrap")
	bootstrap.call("show_status", "Select a bundle in the inspector to unload")


func _on_reload_pressed():
	var bootstrap = get_node("/root/Bootstrap")
	bootstrap.call("show_status", "Select a bundle in the inspector to reload")


func _on_snapshot_pressed():
	var bootstrap = get_node("/root/Bootstrap")
	var snapshot = bootstrap.call("capture_snapshot_dict")
	bootstrap.call(
		"show_status",
		"Snapshot at %s" % snapshot.get("capturedAt", "?"),
	)
