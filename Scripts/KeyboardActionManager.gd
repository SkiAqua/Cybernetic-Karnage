extends ActionManager



func _input(event: InputEvent) -> void:
    if event is InputEventKey and event.is_pressed():
        match event.keycode:
            KEY_J:
                attack.emit()
            KEY_K:
                jump.emit()
            KEY_L:
                special.emit()

func _physics_process(delta: float) -> void:
    direction = Vector2.ZERO

    if Input.is_key_pressed(KEY_A):
        direction.x -= 1
    if Input.is_key_pressed(KEY_D):
        direction.x += 1
    if Input.is_key_pressed(KEY_W):
        direction.y -= 1
    if Input.is_key_pressed(KEY_S):
        direction.y += 1