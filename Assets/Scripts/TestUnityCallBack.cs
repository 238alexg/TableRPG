using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.user;

public class TestUnityCallBack : MonoBehaviour {

	string userName = "Alex G";
	string pwd = "password";
	string emailId = "alexandrgeoffrey@gmail.com";

	// Use this for initialization
	void Start () {
		App42API.Initialize("35f53f9edf5e755bd2f97314ee3cb8924aac582e4b2b36de6515e56137e53451","efa0ed4268abd070c848bf7867083b2641cc304c383f2a23afb8d7c09017defc");

		UserService userService = App42API.BuildUserService();
		userService.CreateUser(userName, pwd, emailId, new UnityCallBack());
	}
}

public class UnityCallBack : App42CallBack  {      
	public void OnSuccess(object response)      {              
		User user = (User) response;              
		Debug.Log("userName is " + user.GetUserName());              
		Debug.Log("emailId is " + user.GetEmail());       
	}      
	public void OnException(System.Exception e)      {              
		Debug.Log("Exception : " + e);      
	}  
}  
