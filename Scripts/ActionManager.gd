extends Node
class_name ActionManager

@export_range(1,2) var player_position: int

var direction: = Vector2.ZERO:
    get:
        return direction.normalized()

signal attack
signal jump
signal special
