extends VBoxContainer


func _ready():
	var bootstrap = get_node("/root/Bootstrap")

	# Wire dock slots
	bootstrap.call("RegisterDockSlot", "inspector", %InspectorSlot)
	bootstrap.call("RegisterDockSlot", "bottom", %BottomSlot)

	# Wire services
	bootstrap.call("SetMenuBar", %EditorMenuBar)
	bootstrap.call("SetStatusLabel", %StatusLabel)
	bootstrap.call("SetFileDialog", %PckFileDialog)

	print("[Shell Bundle] Editor shell ready")
