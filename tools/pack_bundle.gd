## PCK bundle packer tool.
## Usage: godot --headless --script tools/pack_bundle.gd -- <bundle_dir> <output_pck>
##
## Reads manifest.json from bundle_dir, walks the directory tree, and packs
## all files into a PCK with res://bundles/{id}/{relative_path} resource paths.
extends SceneTree


func _init():
	var args = OS.get_cmdline_user_args()
	if args.size() < 2:
		printerr("Usage: godot --headless --script tools/pack_bundle.gd -- <bundle_dir> <output_pck>")
		quit(1)
		return

	var bundle_dir = args[0].replace("\\", "/")
	var output_pck = args[1].replace("\\", "/")

	# Read manifest.json
	var manifest_path = bundle_dir.path_join("manifest.json")
	var file = FileAccess.open(manifest_path, FileAccess.READ)
	if file == null:
		printerr("Cannot read manifest: %s (error: %s)" % [manifest_path, FileAccess.get_open_error()])
		quit(1)
		return

	var json = JSON.parse_string(file.get_as_text())
	file.close()

	if json == null or not json.has("id"):
		printerr("Invalid manifest.json — missing 'id' field")
		quit(1)
		return

	var bundle_id: String = json["id"]
	print("Packing bundle: %s" % bundle_id)

	var packer = PCKPacker.new()
	var err = packer.pck_start(output_pck)
	if err != OK:
		printerr("Failed to start PCK: %s" % error_string(err))
		quit(1)
		return

	var count = _add_dir(packer, bundle_dir, "res://bundles/" + bundle_id)

	packer.flush()
	print("Packed %d files -> %s" % [count, output_pck])
	quit()


func _add_dir(packer: PCKPacker, os_dir: String, res_dir: String) -> int:
	var count := 0
	var dir = DirAccess.open(os_dir)
	if dir == null:
		printerr("Cannot open directory: %s" % os_dir)
		return 0

	dir.list_dir_begin()
	var file_name = dir.get_next()
	while file_name != "":
		if file_name.begins_with("."):
			file_name = dir.get_next()
			continue

		var os_path = os_dir.path_join(file_name)
		var res_path = res_dir + "/" + file_name

		if dir.current_is_dir():
			if file_name == "obj":
				file_name = dir.get_next()
				continue

			if file_name == "bin":
				count += _add_bin_root_files(packer, os_path, res_path)
			else:
				count += _add_dir(packer, os_path, res_path)
		else:
			packer.add_file(res_path, os_path)
			print("  + %s" % res_path)
			count += 1

		file_name = dir.get_next()

	dir.list_dir_end()
	return count


func _add_bin_root_files(packer: PCKPacker, os_dir: String, res_dir: String) -> int:
	var count := 0
	var dir = DirAccess.open(os_dir)
	if dir == null:
		printerr("Cannot open bin directory: %s" % os_dir)
		return 0

	dir.list_dir_begin()
	var file_name = dir.get_next()
	while file_name != "":
		if file_name.begins_with("."):
			file_name = dir.get_next()
			continue

		var os_path = os_dir.path_join(file_name)
		var res_path = res_dir + "/" + file_name

		if not dir.current_is_dir():
			packer.add_file(res_path, os_path)
			print("  + %s" % res_path)
			count += 1

		file_name = dir.get_next()

	dir.list_dir_end()
	return count
