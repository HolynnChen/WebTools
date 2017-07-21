//全局调用，提供集成函数
var x='';
//全局变量
//解释：当数据包过长自动分包时
function getValueFrom(url_string,suc){
  var xhr = new window.XMLHttpRequest();
	if (!window.XMLHttpRequest) {
		try {
			xhr = new window.ActiveXObject("Microsoft.XMLHTTP");
		} catch (e) {}
	}
	xhr.open("get", url_string);
	var oldSize = 0;
	xhr.onreadystatechange = function() {
		if (xhr.readyState == 4 && xhr.status==200) {
			suc(xhr.responseText);
		}
	}
	xhr.send(null);
}//采取ajax进行访问，获取数据，异步

function getValueFrom_times(url_string, fun, suc) {
	var xhr = new window.XMLHttpRequest();
	if (!window.XMLHttpRequest) {
		try {
			xhr = new window.ActiveXObject("Microsoft.XMLHTTP");
		} catch (e) {}
	}
	xhr.open("get", url_string);
	var oldSize = 0;
	xhr.onreadystatechange = function() {
		if (xhr.readyState > 2) {
			var tmpText = xhr.responseText.substring(oldSize);
			oldSize = xhr.responseText.length;
			if (tmpText.length > 0) {
        if(x!=''){
          var y=x;
          x='';
          fun(y+tmpText);
        }else{
				fun(tmpText);
      }
			}
		}
		if (xhr.readyState == 4) {
      x='';
			suc(xhr.responseText);
		}
	}
	xhr.send(null);
}//get方式进行被动轮询

function search_diary(name, time, detail, outputtext, onebyone,delcontent,suc) {
	getValueFrom('./diary.ashx?action=search_diary&name=' + name + '&time=' + time + '&detail=' + detail + '&outputtext=' + outputtext, (data) => {
		if (data == 'false' || data == 'fault') {
			suc('null')
		} else {
			var outputstring = new Array();
			var oip = new Array();
			iop = data.split('|');
			for (var i = 1; i < iop.length; i++) {
				var ccip = new Array();
				ccip = iop[i].split(';');
				var co = iop.length;
				if (detail == '1') {
					outputstring[i - 1] = {
						nianji: ccip[0],
						zubie: ccip[1],
						name: ccip[2],
						id: ccip[3],
						title: LZString.decompressFromBase64(ccip[4].replace(/(success|false|fault)/,''))
					};if(delcontent!='1'){var ss=LZString.decompressFromBase64(ccip[5]);if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				} else {
					outputstring[i - 1] = {
						name: ccip[0],
						id: ccip[1],
						title: LZString.decompressFromBase64(ccip[2].replace(/(success|false|fault)/,''))
					};if(delcontent!='1'){var ss=LZString.decompressFromBase64(ccip[3]);if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				}
			};
			suc(outputstring)
		}
	})
}

function search_diary_times(name, time, detail, outputtext,onebyone,delcontent, fun,suc) {
	getValueFrom_times('./diary.ashx?action=search_diary&name=' + name + '&time=' + time + '&detail=' + detail + '&outputtext=' + outputtext+'&onebyone='+onebyone+'&delcontent='+delcontent, (data) => {
		if (data == 'false' || data == 'fault') {
			fun('null')
		} else {
			var outputstring = new Array();
			var oip = new Array();
			iop = data.split('|');
			for (var i = 1; i < iop.length; i++) {
				var ccip = new Array();
				ccip = iop[i].split(';');
				var co = iop.length;
				if (detail == '1') {
					outputstring[i - 1] = {
						nianji: ccip[0],
						zubie: ccip[1],
						name: ccip[2],
						id: ccip[3],
						title: LZString.decompressFromBase64(ccip[4].replace(/(success|false|fault)/,''))
					};if(delcontent!='1'){var ss=LZString.decompressFromBase64(ccip[5]);if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				} else {
					outputstring[i - 1] = {
						name: ccip[0],
						id: ccip[1],
						title: LZString.decompressFromBase64(ccip[2].replace(/(success|false|fault)/,''))
					};{var ss=LZString.decompressFromBase64(ccip[3]);if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				}
			};
			fun(outputstring)
		}
	},suc)
}

function search_arc_times(searchkey, searchway, searchnumber, delcontent, detail, fun, suc) {
	getValueFrom_times('./diary.ashx?action=search_arc&searchkey=' + searchkey + '&searchway=' + searchway + '&searchnumber=' + searchnumber + "&delcontent=" + delcontent + "&detail=" + detail, (data) => {
		if (data == 'false' || data == 'fault' || data == '参数错误' || data == 'no_arc') {
			fun('null')
		} else {
			var outputstring = new Array();
			var oip = new Array();
			iop = data.split('|');
			for (var i = 1; i < iop.length; i++) {
				var ccip = new Array();
				ccip = iop[i].split(';');
				var co = iop.length;
				if (detail == '1') {
					outputstring[i - 1] = {
						nianji: ccip[0],
            zubie: ccip[1],
						name: ccip[2],
						id: ccip[3],
						node_name: ccip[4],
            title: LZString.decompressFromBase64(ccip[5].replace(/(success|false|fault)/, ''))
					};
					if (delcontent != '1') {var ss=LZString.decompressFromBase64(ccip[6]);if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				} else {
					outputstring[i - 1] = {
						name: ccip[0],
						id: ccip[1],
						node_name: ccip[2],
						title: LZString.decompressFromBase64(ccip[3].replace(/(success|false|fault)/, ''))
					};
					if (delcontent != '1') {var ss=LZString.decompressFromBase64(ccip[4]);if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				}
			};
			fun(outputstring)
		}
	}, suc)
}

function put_yzm(string,fun){
  getValueFrom('./code.ashx?action=login&yzm='+string,fun);
}
