extends VBoxContainer


func _ready():
	print("[Inspector Bundle] ready")
	_populate_tree()
	call_deferred("_reparent_to_hud")


func _populate_tree():
	var tree = $Tree
	tree.columns = 1
	var root_item = tree.create_item()
	root_item.set_text(0, "Inspector Bundle")

	var section = tree.create_item(root_item)
	section.set_text(0, "Sample Data")

	for i in range(3):
		var item = tree.create_item(section)
		item.set_text(0, "Item %d" % (i + 1))


func _reparent_to_hud():
	var target = get_node_or_null("/root/Main/HudRoot/InspectorPanel")
	if target:
		reparent(target)
		print("[Inspector Bundle] reparented to InspectorPanel")
	else:
		push_warning("[Inspector Bundle] InspectorPanel not found")
