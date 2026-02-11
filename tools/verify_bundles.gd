## Verify PCK bundles can be mounted and scenes loaded.
## Usage: godot --path project/hosts/complete-app --headless --script ../../../tools/verify_bundles.gd -- <pck1> [pck2] ...
extends SceneTree

var _errors := 0


func _init():
	var args = OS.get_cmdline_user_args()
	if args.size() == 0:
		printerr("Usage: godot --headless --script tools/verify_bundles.gd -- <pck_file> ...")
		quit(1)
		return

	for pck_path in args:
		_verify_pck(pck_path)

	if _errors > 0:
		printerr("\nFAILED: %d error(s)" % _errors)
		quit(1)
	else:
		print("\nAll bundles verified OK")
		quit()


func _verify_pck(pck_path: String):
	print("\n=== Verifying: %s ===" % pck_path)

	# 1. Mount PCK
	if not ProjectSettings.load_resource_pack(pck_path, true):
		printerr("  FAIL: Could not mount PCK")
		_errors += 1
		return
	print("  OK: PCK mounted")

	# 2. Derive bundle ID from filename
	var bundle_id = pck_path.get_file().get_basename()
	var manifest_path = "res://bundles/%s/manifest.json" % bundle_id

	# 3. Check manifest exists
	if not FileAccess.file_exists(manifest_path):
		printerr("  FAIL: Manifest not found at %s" % manifest_path)
		_errors += 1
		return
	print("  OK: Manifest found at %s" % manifest_path)

	# 4. Parse manifest
	var file = FileAccess.open(manifest_path, FileAccess.READ)
	var json = JSON.parse_string(file.get_as_text())
	file.close()

	if json == null or not json.has("id"):
		printerr("  FAIL: Invalid manifest")
		_errors += 1
		return
	print("  OK: Manifest parsed — id=%s, displayName=%s" % [json["id"], json.get("displayName", "?")])

	# 5. Load root scene if specified
	var root_scene_path = json.get("rootScene")
	if root_scene_path == null:
		print("  SKIP: No rootScene in manifest")
		return

	var scene = ResourceLoader.load(root_scene_path) as PackedScene
	if scene == null:
		printerr("  FAIL: Could not load scene at %s" % root_scene_path)
		_errors += 1
		return
	print("  OK: Scene loaded from %s" % root_scene_path)

	# 6. Instantiate scene
	var instance = scene.instantiate()
	if instance == null:
		printerr("  FAIL: Could not instantiate scene")
		_errors += 1
		return
	print("  OK: Scene instantiated — node=%s type=%s" % [instance.name, instance.get_class()])

	# Cleanup
	instance.queue_free()
