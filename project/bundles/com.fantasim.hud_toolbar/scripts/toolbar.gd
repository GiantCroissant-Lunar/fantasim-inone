extends HBoxContainer


func _ready():
	print("[Toolbar Bundle] ready")
	call_deferred("_reparent_to_hud")


func _reparent_to_hud():
	var target = get_node_or_null("/root/Main/HudRoot/InspectorPanel")
	if target:
		var existing_toolbar = target.get_node_or_null("Toolbar")
		reparent(target)
		if existing_toolbar:
			target.move_child(self, existing_toolbar.get_index())
		print("[Toolbar Bundle] reparented to InspectorPanel")
	else:
		push_warning("[Toolbar Bundle] InspectorPanel not found")
