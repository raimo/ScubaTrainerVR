/*
Floating Object Script   
6/30/2014
Brendan Dickinson
This script was made for a game called Big Fish 3D to simulate underwater floating
the game was made by a group of university students if you would like to see 
the game check out www.bigfish3d.com
*/

#pragma strict

private var sine : float = 0.0;
private var sw : int = 0; //switch needed during sine curve
private var timer : float = 0.0;
private var xMovement : float;
private var xtorque : float;    //turns the object on the x axis
private var ytorque : float;    //turns the object on the y axis
private var ztorque : float;	//turns the object on the z axis
var verticalSpeed : float = 1.0;
var verticalDistance : float = 1.0;
var horizontalSpeed : float = 1.0;
var spinSpeed : float = 1.0;

function Start () {		//Only executes when the program starts
    xMovement = Random.Range(-.5,.5)*horizontalSpeed;   //random value between -0.5 and 0.5, causing some movement on the x axis.
    xtorque = Random.Range(-5.0,5.0)*spinSpeed;	//turns the object on the x axis
    ytorque = Random.Range(-5.0,5.0)*spinSpeed;	//turns the object on the y axis
    ztorque = Random.Range(-5.0,5.0)*spinSpeed;	//turns the object on the z axis
    GetComponent.<Rigidbody>().AddRelativeTorque(Vector3(xtorque, ytorque, ztorque));	//function to actually add the turning on all three axis
}

function FixedUpdate () {   //FixedUpdate is used because it is used with physics

	if(sine < Mathf.PI && sw == 0){	//sine variable is fluctuating between 0 and Pi causing an up and down motion simulating floating, think sine curve
	sine += Time.deltaTime;
	}
	if(sine >= Mathf.PI){
		sw = 1;
		}
	if(sine <= 0){
		sw = 0;
		}
	if(sine >= 0 && sw == 1){
	sine = 0;
	}	
		
	GetComponent.<Rigidbody>().velocity = Vector3(xMovement, Mathf.Sin(2*sine*verticalSpeed)*verticalDistance, 0);	//Adds the x axis movement and up and down motion to the object
	
	if(timer < 10){ //increments timer
		timer += Time.deltaTime;
	}
	if(timer >= 10){    //This adds the torque that was executed at the start again every 10 seconds to have the object continue to turn slightly.
		timer = 0;
		GetComponent.<Rigidbody>().AddRelativeTorque(Vector3(xtorque, ytorque, ztorque));	//Adds the torque on all axis again. Does not compute new numbers just continues previous ones.
	}
}
