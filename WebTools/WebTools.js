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
			var iop = new Array();
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
					};if (delcontent != '1') {if(ccip[5].charAt(ccip[5].length-1)!='-'){x = data;return};var ss=LZString.decompressFromBase64(ccip[5].substring(0,ccip[5].length-1));if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
				} else {
					outputstring[i - 1] = {
						name: ccip[0],
						id: ccip[1],
						title: LZString.decompressFromBase64(ccip[2].replace(/(success|false|fault)/,''))
					};if (delcontent != '1') {if(ccip[3].charAt(ccip[3].length-1)!='-'){x = data;return};var ss=LZString.decompressFromBase64(ccip[3].substring(0,ccip[3].length-1));if(ss!=""){outputstring[i - 1].content = ss}else{x = data;return}}
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
			var iop = new Array();
			iop = data.split('|');
			if(iop.length>1) {
				var ccip = new Array();
				ccip = iop[i].split(';');
				var co = iop.length;
				if (detail == '1') {
					outputstring= {
						nianji: ccip[0],
						zubie: ccip[1],
						name: ccip[2],
						id: ccip[3],
						title: LZString.decompressFromBase64(ccip[4].replace(/(success|false|fault)/,''))
					};if (delcontent != '1') {if(ccip[5].charAt(ccip[5].length-1)!='-'){x = data;return};var ss=LZString.decompressFromBase64(ccip[5].substring(0,ccip[5].length-1));if(ss!=""){outputstring.content = ss}else{x = data;return}}
				} else {
					outputstring= {
						name: ccip[0],
						id: ccip[1],
						title: LZString.decompressFromBase64(ccip[2].replace(/(success|false|fault)/,''))
					};if (delcontent != '1') {if(ccip[3].charAt(ccip[3].length-1)!='-'){x = data;return};var ss=LZString.decompressFromBase64(ccip[3].substring(0,ccip[3].length-1));if(ss!=""){outputstring.content = ss}else{x = data;return}}
				}
			};
			fun(outputstring)
		}
	},suc)
}

function search_arc_times(searchkey, searchway, searchnumber, delcontent, detail, dc_put, fun, suc) {
	getValueFrom_times('./diary.ashx?action=search_arc&searchkey=' + searchkey + '&searchway=' + searchway + '&searchnumber=' + searchnumber + "&delcontent=" + delcontent + "&detail=" + detail + "&dc_put=" + dc_put, (data) => {
		if (data == 'false' || data == 'fault' || data == '参数错误' || data == 'no_arc') {
			fun('null')
		} else {
			if (dc_put == '1') {
				var iop = new Array();
				iop = data.split('|');
				if (iop.length > 1) {
					var outputstring = iop[1];
				} else {
					outputstring = 'null'
				}
			} else {
				var outputstring = new Array();
				var iop = new Array();
				iop = data.split('|');
				if (iop.length > 1) {
					var i = 1;
					var ccip = new Array();
					ccip = iop[i].split(';');
					var co = iop.length;
					if (detail == '1') {
						outputstring = {
							nianji: ccip[0],
							zubie: ccip[1],
							name: ccip[2],
							id: ccip[3],
							node_name: ccip[4],
							title: LZString.decompressFromBase64(ccip[5].replace(/(success|false|fault)/, ''))
						};
						if (delcontent != '1') {
							if (ccip[6].charAt(ccip[6].length - 1) != '-') {
								x = data;
								return
							};
							var ss = LZString.decompressFromBase64(ccip[6].substring(0, ccip[6].length - 1));
							if (ss != "") {
								outputstring.content = ss
							} else {
								x = data;
								return
							}
						}
					} else {
						outputstring = {
							name: ccip[0],
							id: ccip[1],
							node_name: ccip[2],
							title: LZString.decompressFromBase64(ccip[3].replace(/(success|false|fault)/, ''))
						};
						if (delcontent != '1') {
							if (ccip[4].charAt(ccip[4].length - 1) != '-') {
								x = data;
								return
							};
							var ss = LZString.decompressFromBase64(ccip[4].substring(0, ccip[4].length - 1));
							if (ss != "") {
								outputstring.content = ss
							} else {
								x = data;
								return
							}
						}
					}
				}
			};
			fun(outputstring)
		}
	}, suc)
}

function put_yzm(string,fun){
  getValueFrom('./code.ashx?action=login&yzm='+string,fun);
}
