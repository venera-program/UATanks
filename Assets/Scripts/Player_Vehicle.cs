﻿using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Player_Vehicle : MonoBehaviour, IVehicle
{
	// IVehicle: Event is triggered when action condition is met
	public event System.Action Action_Primary;
	// IVehicle: Event is triggered when action condition is met
	public event System.Action Action_Secondary;
	// IVehicle: Event is triggered when action condition is met
	public event System.Action Action_Death;

	// IVehicle: Property for local variable VehicleData vehicleData
	public VehicleData Data { get { return this.vehicleData; } }
	// IVehicle: Property for local variable int currentHealth
	public int Health { get { return this.currentHealth; } }

	[Header("Movement Data")]
	[SerializeField, Tooltip("Reference to VehicleData Asset which provides vehicle information.")]
	// Local reference to this vehicle's information asset.
	private VehicleData vehicleData;

	// Used to track the current health of the vehicle.
	private int currentHealth;
	// Local reference to CharacterController component.
	private CharacterController controller;
	// Local reference to Transform component.
	private Transform tf;

	private void Awake()
	{
		this.controller = this.GetComponent<CharacterController>();
		this.tf = this.GetComponent<Transform>();
	}

	private void Start()
	{
		this.currentHealth = this.Data.MaxHealth;

		this.Action_Death += () =>
			{
				GameManager.KillPlayer();
				this.StopAllCoroutines();
				Destroy(this.gameObject);
			};
	}

	private void OnEnable()
	{
		this.StartCoroutine("MovementInput");
	}

	private void OnDisable()
	{
		this.StopCoroutine("MovementInput");
	}

	private IEnumerator MovementInput()
	{
		while (true)
		{
			this.CalculateMovement();

			if (Input.GetButtonDown("Primary Action"))
			{
				if (this.Action_Primary != null)
				{
					this.Action_Primary();
				}
			}
			else if (Input.GetButtonDown("Secondary Action"))
			{
				if (this.Action_Secondary != null)
				{
					this.Action_Secondary();
				}
			}

			yield return null;
		}
	}

	private void CalculateMovement()
	{
		float horzAxis = Input.GetAxis("Horizontal");
		float vertAxis = Input.GetAxis("Vertical");

		if (vertAxis > 0)
		{
			vertAxis *= this.Data.ForwardSpeed;
		}
		else
		{
			vertAxis *= this.Data.ReverseSpeed;
		}

		this.tf.Rotate(0, horzAxis * this.Data.RotateSpeed * Time.deltaTime, 0);
		this.controller.SimpleMove(vertAxis * this.tf.forward);
	}

	/// <summary>
	/// IVehicle: Damages the vehicle based on given parameters.
	/// </summary>
	/// <param name="amount">Amount of damage being taken.</param>
	public void TakeDamage(int amount)
	{
		// Decrements health
		this.currentHealth -= amount;

#if UNITY_EDITOR
		Debug.Log(this.name + "'s Health is now = " + this.currentHealth, this);
#endif

		// if: Current health is below threshhold
		if (this.currentHealth <= 0)
		{
			// Call death event
			this.Action_Death();
		}
	}
}
