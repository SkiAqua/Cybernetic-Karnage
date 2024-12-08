using Godot;
using System;

[GlobalClass]
public partial class Character : CharacterBody2D
{   
	//Enums
	public enum Instance
	{
		idle,
		walking,
		jumping,
		attacking
	}
	//Constants
	private const string IdleAnimation = "idle";
	private const string WalkingAnimation = "walking";
	private const string JumpAnimation = "jump";
	private const string JumpingAnimation = "jumping";
	//Vars
	public Instance instance = Instance.idle;

	private bool _jumping = false;
	private float _down_acceleration = 0;

	private float _moveSpeed = 16000f;
	
	private  int _facing = 1;
	private float _walking_animation_speed = 1;
	
	public float Height = 0;
	public Vector2 Direction;
	// Exports
	[Export] private GDScript _controlData;

	[Export(PropertyHint.Range, "1,3")] private int _controlPlayer;

	[Export] public float Gravity = 2600f;
	[Export] public float JumpForce = 700;
	[Export] public float JumpDashForce = 300;

	[Export] public float MoveSpeed
	{
		get { return _moveSpeed;}
		set 
		{
			_moveSpeed = value;
			_walking_animation_speed = value/16800f;
		}
	}
	
	public bool Mirrored
	{
		get => _facing == -1;
		set => _facing = value ? -1 : 1;
	}
	[Export] private NodePath _characterNodePath;
	//Nodes
	public AnimationPlayer animator;
    public Node2D graphicsNode;
	public Node2D characterNode;
	//Custom Nodes
	public Node control;
	public Timer motionTimer;

	public override void _Ready()
	{
		base._Ready();
		//Inicializar variáveis
		
		//Inicializar Nodes
		control = (Node)_controlData.New();
		motionTimer = new Timer();

		characterNode = GetNode<Node2D>(_characterNodePath);
		animator = GetNode<AnimationPlayer>("Animator");
        graphicsNode = GetNode<Node2D>("Gfx");

		//Sinais e add_child
		control.Connect("jump",Callable.From(Jump));
		AddChild(control);
	}
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		_Movement((float) delta);
		_Gravity((float) delta);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		_ScanState();
		_UpdateAnimation();
		_Mirroring();
    }
    public void Jump()
	{	
		if (_jumping)
			return;

		_jumping = true;
		_down_acceleration = -JumpForce;
	}

	private void _Movement(float delta)
	{
		if (!_jumping)
		{
			Direction = (Vector2)control.Get("direction");
		}

		if ( Direction != Vector2.Zero ) //IsMoving
		{
			Velocity = Direction * delta * MoveSpeed;
		} else //IsNotMoving
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}

	private void _Gravity(float delta)
	{
		if (_jumping)
		{
			_down_acceleration += Gravity * delta;
			Height = Mathf.Clamp(Height - _down_acceleration * delta, 0f, 9999f);
		
			characterNode.Position = new Vector2(0, -Height);

			_jumping = Height > 0f;
		}

	}

	private void _ScanState()
	{
		if (_jumping)
		{
			instance = Instance.jumping;
			return;
		}
		if (Direction != Vector2.Zero)
		{
			instance = Instance.walking;
			return;
		}

		instance = Instance.idle;
	}

	private void _UpdateAnimation()
	{
		switch (instance)
		{
			case Instance.walking:
				if (animator.CurrentAnimation != WalkingAnimation)
				{
					animator.Play(WalkingAnimation);
					animator.SpeedScale = _walking_animation_speed;
				}
				break;
			
			case Instance.idle:
				if (animator.CurrentAnimation != IdleAnimation)
				{
					animator.Play(IdleAnimation);
				}
				break;
			
			case Instance.jumping:
				if (animator.CurrentAnimation != JumpAnimation && animator.CurrentAnimation != JumpingAnimation)
				{
					animator.Play(JumpAnimation);
				}
				break;
		}
	}

	private void _Mirroring()
	{
		if (Velocity.X > 0)
			_facing = 1;
		else if (Velocity.X < 0)
			_facing = -1;
		
		GetNode("Gfx").Set("scale", new Vector2(_facing,1));
		GetNode<Area2D>("DamageArea").Scale = new Vector2(_facing, 1);
		GetNode<Area2D>("DangerArea").Scale = new Vector2(_facing, 1);
	}
}
