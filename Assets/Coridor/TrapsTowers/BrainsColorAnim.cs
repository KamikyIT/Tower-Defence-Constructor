using System;
using System.Collections.Generic;
using UnityEngine;

public class BrainsColorAnim : MonoBehaviour
{
	private SpriteRenderer sprite;

	public int BrainsMax = 100;
	public BrainsState State = BrainsState.Green;

	public bool rising;

	public Color Green1;
	public Color Green2;

	public Color LowGreen1;
	public Color LowGreen2;

	public Color Yellow1;
	public Color Yellow2;

	public Color Orange1;
	public Color Orange2;

	public Color Red1;
	public Color Red2;

	public Color BlackRed1;
	public Color BlackRed2;

	public Color CurrentMin;
	public Color CurrentMax;

	private Color delta;
	private Color deltaR;
	private Color deltaG;
	private Color deltaB;

	// Use this for initialization
	void Start ()
	{
		sprite = GetComponent<SpriteRenderer>();

		rising = true;

	}

	void OnGUI()
	{
		return;

		var i = 0;
		if (GUI.Button(new Rect(10, 10, 150, 100), ((BrainsState)i).ToString()))
			SetState(((BrainsState)i));
		i++;

		if (GUI.Button(new Rect(10, 10 + 100 * i, 150, 100), ((BrainsState)i).ToString()))
			SetState(((BrainsState)i));
		i++;

		if (GUI.Button(new Rect(10, 10 + 100 * i, 150, 100), ((BrainsState)i).ToString()))
			SetState(((BrainsState)i));
		i++;

		if (GUI.Button(new Rect(10, 10 + 100 * i, 150, 100), ((BrainsState)i).ToString()))
			SetState(((BrainsState)i));
		i++;

		if (GUI.Button(new Rect(10, 10 + 100 * i, 150, 100), ((BrainsState)i).ToString()))
			SetState(((BrainsState)i));
		i++;

		if (GUI.Button(new Rect(10, 10 + 100 * i, 150, 100), ((BrainsState)i).ToString()))
			SetState(((BrainsState)i));
		i++;
	}

	// Update is called once per frame
	void Update ()
	{
		var color = sprite.color;

		var hasTodo = false;

		if (Math.Abs(color.r - CurrentMax.r) > delta.r)
		{
			hasTodo = true;

			if (color.r > CurrentMax.r)
				color.r -= delta.r;
			else
				color.r += delta.r;
		}

		if (Math.Abs(color.g - CurrentMax.g) > delta.g)
		{
			hasTodo = true;

			if (color.g > CurrentMax.g)
				color.g -= delta.g;
			else
				color.g += delta.g;
		}

		if (Math.Abs(color.b - CurrentMax.b) > delta.b)
		{
			hasTodo = true;

			if (color.b > CurrentMax.b)
				color.b -= delta.b;
			else
				color.b += delta.b;
		}

		if (hasTodo == false)
		{
			var temp = CurrentMin;
			CurrentMin = CurrentMax;
			CurrentMax = temp;
		}

		color.a = 1f;

		sprite.color = color;
	}

	public void BrainsChanged(int brains)
	{
		var pers = (float)brains / (float)BrainsMax;

		if (pers > 0.95f)
			SetState(BrainsState.Green);
		else if (pers > 0.8f)
			SetState(BrainsState.LowGreen);
		else if(pers > 0.6f)
			SetState(BrainsState.Yellow);
		else if(pers > 0.4f)
			SetState(BrainsState.Orange);
		else if(pers > 0.2f)
			SetState(BrainsState.Red);
		else
			SetState(BrainsState.BlackRed);
	}

	private void SetState(BrainsState state)
	{
		if (State == state)
			return;

		this.State = state;

		rising = true;

		switch (state)
		{
			case BrainsState.Green:
				CurrentMin = Green1;
				CurrentMax = Green2;
				break;
			case BrainsState.LowGreen:
				CurrentMin = LowGreen1;
				CurrentMax = LowGreen2;
				break;
			case BrainsState.Yellow:
				CurrentMin = Yellow1;
				CurrentMax = Yellow2;
				break;
			case BrainsState.Orange:
				CurrentMin = Orange1;
				CurrentMax = Orange2;
				break;
			case BrainsState.Red:
				CurrentMin = Red1;
				CurrentMax = Red2;
				break;
			case BrainsState.BlackRed:
				CurrentMin = BlackRed1;
				CurrentMax = BlackRed2;
				break;
			default:
				throw new ArgumentOutOfRangeException("green", state, null);
		}

		sprite.color = CurrentMin;

		delta = (CurrentMax - CurrentMin);

		delta = new Color(Math.Abs(delta.r), Math.Abs(delta.g), Math.Abs(delta.b));

		delta *= 0.1f;
	}

	public enum BrainsState
	{
		Green,
		LowGreen,
		
		Yellow,

		Orange,

		Red,
		BlackRed,
	}
}
