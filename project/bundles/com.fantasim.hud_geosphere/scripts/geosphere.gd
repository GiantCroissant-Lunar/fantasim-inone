extends VBoxContainer

const CHANNEL := "geosphere.plates.summary"


func _ready():
	var bootstrap = get_node_or_null("/root/Bootstrap")
	if bootstrap == null:
		%StatusLabel.text = "Status: bootstrap not found"
		return

	bootstrap.connect("HudDataChanged", _on_hud_data_changed)
	bootstrap.connect("TickScrubbed", _on_tick_scrubbed)
	%Details.text = "Waiting for geosphere data events..."


func _on_tick_scrubbed(tick: int, is_scrubbing: bool):
	var suffix = " (scrubbing)" if is_scrubbing else ""
	%TickLabel.text = "Tick: %d%s" % [tick, suffix]


func _on_hud_data_changed(channel: String, payload_json: String):
	if channel != CHANNEL:
		return

	var parsed = JSON.parse_string(payload_json)
	if typeof(parsed) != TYPE_DICTIONARY:
		%StatusLabel.text = "Status: invalid payload"
		%Details.text = payload_json
		return

	_render_payload(parsed)


func _render_payload(payload: Dictionary):
	var status = str(payload.get("Status", payload.get("status", "unknown")))
	var message = str(payload.get("Message", payload.get("message", "")))
	var stream = str(payload.get("StreamIdentity", payload.get("streamIdentity", "")))
	var tick = int(payload.get("Tick", payload.get("tick", -1)))
	var last_sequence = int(payload.get("LastSequence", payload.get("lastSequence", -1)))
	var counts: Dictionary = payload.get("Counts", payload.get("counts", {}))
	var plates: Array = payload.get("Plates", payload.get("plates", []))

	%StatusLabel.text = "Status: %s" % status

	var lines: Array[String] = []
	lines.append(message)

	if stream != "":
		lines.append("Stream: %s" % stream)
	if tick >= 0:
		lines.append("Materialized Tick: %d" % tick)
	if last_sequence >= 0:
		lines.append("Last Sequence: %d" % last_sequence)

	if not counts.is_empty():
		lines.append(
			"Counts: plates=%d active=%d boundaries=%d active=%d junctions=%d active=%d"
			% [
				int(counts.get("Plates", counts.get("plates", 0))),
				int(counts.get("ActivePlates", counts.get("activePlates", 0))),
				int(counts.get("Boundaries", counts.get("boundaries", 0))),
				int(counts.get("ActiveBoundaries", counts.get("activeBoundaries", 0))),
				int(counts.get("Junctions", counts.get("junctions", 0))),
				int(counts.get("ActiveJunctions", counts.get("activeJunctions", 0))),
			]
		)

	if plates.size() > 0:
		lines.append("")
		lines.append("Plates:")
		for plate in plates:
			if typeof(plate) == TYPE_DICTIONARY:
				var plate_id = str(plate.get("PlateId", plate.get("plateId", "?")))
				var retired = bool(plate.get("IsRetired", plate.get("isRetired", false)))
				lines.append("- %s%s" % [plate_id, " (retired)" if retired else ""])
			else:
				lines.append("- %s" % str(plate))

	%Details.text = "\n".join(lines)
