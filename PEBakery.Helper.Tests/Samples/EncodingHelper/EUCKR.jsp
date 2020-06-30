




<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
	<meta http-equiv="X-UA-Compatible" content="IE=edge">
	<meta http-equiv="Content-Language" content="ko">
	<meta http-equiv="imagetoolbar" content="no">
	<meta http-equiv="Content-Type" content="text/html; charset=euc-kr">
	<title>����� ������û</title>
	<link href="/style/sugang.css" rel="stylesheet" type="text/css">
	<style type="text/css">
	* { font-family:AppleGothic, NanumGothic, �������, "Malgun Gothic", "���� ���", Dotum, ����, Tahoma, Sans-serif; font-size:12px; }
	img { border:0; }
	#top { height:52px; background: url("images/tit_bg.gif"); padding:15px 0 0 20px; }
	
	.input_login { border:#ccccc5 1px solid; background-color:#e1e1d4; padding-left:3px; height:15px; width:130px; }
	
	#loginBox { margin-top:10px; margin-left:20%; }
	#loginBox fieldset { 
		width:440px; 
		border:5px solid #e6e3d5;
		background:#f2f2ea;
		padding:10px 0px 10px 0px;
	}
	#loginBox dl { float:left; overflow:hidden; width:280px; text-align:right; }
	#loginBox dt { float:left; width:140px; margin-top:5px; }
	#loginBox dd { margin-bottom: 5px; }
	#loginButton { float:left; margin-left:5px; }
	#loginButton ul{ list-style:none; padding:0px; }

	#sugangGuid { margin-left:10%; margin-top:20px; }
	#sugangGuid ul {
		margin: 0;
		padding: 0;
		list-style: none;
	}
	#sugangGuid li {
		display:block;
		padding:5px 0 5px 15px;
		background: url(images/bullet_logtxt.gif) no-repeat left center;
		text-align:left;
	}
	
	#sugangGuid li.underline{
		text-decoration: underline;
	}
	
	#sugangGuid li.innerline{
		margin-left:53px;
		background: url(images/new/bul/bul_01.png) no-repeat left center; 
	}
	
	.tit_redbullet{display:block;min-height:16px;padding-left:20px;margin:25px 0 0 0;font:bold 12px/17px Dotum;color:#000;background:url(/images/new/bul/bul_32.png) no-repeat left top}
	.inner_cont{padding-left: 35px;}
	</style>

<script type="text/javascript" src="/js/netfunnel.js" charset="utf-8"></script>
<script type="text/javascript">
	function login(obj){
	   strID = loginForm.id.value;
	   if(strID.length == 0 || strID == " "){
	      	alert("����� ID�� �Էµ��� �ʾҽ��ϴ�.");
	    	document.getElementById('id').focus();      
		  	return;
	   }
	   strPW = loginForm.pw.value;
	   if(strPW.length == 0 || strPW == " "){
			alert("��ȣ�� �Էµ��� �ʾҽ��ϴ�.");
			return;
	   } else {
			loginForm.action = "https://sugang.korea.ac.kr/LoginAction.jsp?menuDiv=1";
			//loginForm.submit();
			//netfunnel_comment [1_2_1] login 
			NetFunnel_Action({action_id: "login",popup_target:top.firstF, use_frame_block:true, frame_block_list:[{win:top.secondF}]},loginForm);
		}
	}

	function f_open3(){
		window.open("./lecture/time_table.htm","new_notice","width=850,height=700,top=0,left=100,toolbar=no,status=no,menubar=no,scrollbars=yes,resizable=yes");
	}

	function  openNewWindow(){
	   var url = '/course/MedicalExamInfo.htm';
	   var imywidth;
	   var imyheight;
	   imywidth = (window.screen.width/2) - (247+10);
	   imyheight = (window.screen.height/2) - (185+50);
	   var newwin = window.open(url,"_blank", "scrollbars=no ,resizable=no,copyhistory=no,width=550, height=400" + ",left=" + imywidth + ",top=" + imyheight);
	}

	function  openNotice(){
		alert("���� ���� 10�ú��� 10�� 45�б��� �ο����Ѱ����� ������û�Ͻ� �� �������ϴ�. \n\n10�� 45�� ����� ������ �����Ǿ� �ο����Ѱ����û�� �����������ϴ�.\n\n��뿡 ������ ����� ����� �˼��մϴ�.");
	}

	function f_calls(str) {
		switch (str) {
			case 0 :
				window.open("http://sugang.korea.ac.kr/elective_info0001.htm", "elective information", "left=0, top=0, width=620, height=530,resizable=no,status=no,toolbar=no,menubar=no,scrollbars=no");
				break;
			case 1 :
				window.open("https://infodepot.korea.ac.kr/student/exchangestudent/psearch.jsp", "exchangestudent", "left=0, top=0, width=620, height=530,resizable=no,status=no,toolbar=no,menubar=no,scrollbars=no");
				break;
			default:;
		}
	}
	
	function f_onload(){
		document.getElementById('id').focus();
	}
</script>
</head>
<body onload="f_onload();" >
<div id="wrap">
<div id="top" align="left">

	<img src="images/tit_app_sug.gif" alt="�кμ�����û�ý���" width="170" height="21">

</div>



	<div id="loginBox">
		<form method="post" name="loginForm">
		<fieldset>
			<dl>
				<dt><label for="id"><img src="images/login_num.gif" alt="�й�" width="45" height="12"></label></dt>
				<dd><input type="text" id="id" name="id" value="" class="input_login"></dd>
				<dt><label for="pw"><img src="images/login_pw.gif" alt="��ȣ" width="45" height="12"></label></dt>
				<dd><input type="password" id="pw" name="pw" value="" class="input_login" onkeydown="if(event.keyCode==13) login();"></dd>
			</dl>
			<div id="loginButton">
				<ul>
					<li><a href="#" onclick="login()"><img src="images/btn_login.gif" alt="login" border="0"></a></li>
				</ul>
			</div>
		</fieldset>
		</form>
	</div>

	<div id="sugangGuid">
		<!-- ������û �ȳ����� -->
		<span class="tit_redbullet">������û �ý��� ���ȳ�</span>
		<div class="inner_cont">
			<ul>
				<li><a target="_blank" href="./notice_macro.htm"><span style="font-weight:bold; color:red; text-decoration:underline;">������û�ý��� �ߺ��α���/��ũ�� ���� ��� ���� �ȳ�</span></a></li>
				<li><a target="_blank" href="./guide.htm"><span style="font-weight:bold; color:red; text-decoration:underline;">�α����� ���� ���� ��, Internet Explorer �������� �ȳ�</span></a></li>
				<li><span class="font_red" style="color:red;">����������� ��ϱⰣ �ȳ� -    2. 4(ȭ) 10:00 - 2. 7(��) 12:00</span></li>
				<li>�л���� �ֿ���� �ȳ��� �������� Ȩ�������� �����ϼ���. <a target="_blank" href="http://registrar.korea.ac.kr"><span style="font-weight:bold; color: red;text-decoration:underline;">�������� �ٷΰ���</span></a></li>
				<li>ȭ����� �޴��� �ִ� ������û�ȳ�, �ܴ뺰 ������û���ǻ����� �����Ͻ� �� ������û�� �Ͻʽÿ�.</li>
				<li><span class="bold">��ȣ</span> - ����(KUPID)����� : ���к�й�ȣ</li>
				<li class="innerline">����(KUPID)�̻���� : '���й̻���� ��й�ȣ����'���� ������ ��й�ȣ(������: �ֹι�ȣ���ڸ�)</li>
				<li class="innerline">(���л������ ���Ի��� ���������� ���й̻���ڿ� �ش��ϴ� ��й�ȣ ���)</li>				
				<li>��ȣ �нǽ� - ���� ����� : ����(http://portal.korea.ac.kr) �α��� ȭ���� '��й�ȣã��'���� ��й�ȣ ��߱�</li>
				<li class="innerline">����(KUPID)�̻���� : ������û(http://sugang.korea.ac.kr) '���й̻���� ��й�ȣ����' �޴����� ��й�ȣ ��߱�</li>
				<li class="innerline"><span class="font_red" style="font-weight:bold;">���п��� ��й�ȣ�� ���� �Ǵ� ��߱� ���� ���� 10���Ŀ� �α��� �ϱ� �ٶ��ϴ�.</span></li>
				<li class="underline"><a href="#" onClick="f_calls(1)">���� ���� �л��� �й� Ȯ��</a></li>
				<li>Internet Explorer 9.0 �̻��� ����, ȭ�� �ػ� 1024*768�� ����ȭ �Ǿ� �ֽ��ϴ�.</li>
			</ul>
		</div>		
	</div>
</div>
<br/>
<br/>
</body>
</html>
