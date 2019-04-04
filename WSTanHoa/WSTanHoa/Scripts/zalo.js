function getContext() {
        ZaloExtensions.getContext({ "oaId": 513683145569486355 },
        function success(thread_context) {
            //console.log(thread_context);
            var result = "[" + JSON.stringify(thread_context) + "]";
            var objjson = JSON.parse(result);
            //alert(objjson[0]['userId'])
            $('#IDZalo').val(objjson[0]['userId'])
            //var option = {
            //    url: '/Zalo/Index',
            //    data: JSON.stringify({ id: 1 }),
            //    method: 'post',
            //    dataType: 'json',
            //    contentType: 'application/json;charset=utf-8'
            //};
            //$.ajax(option).success();
        },
                function error(err) {
                    //console.log(err);
                    var result = "[" + JSON.stringify(err) + "]";
                    var objjson = JSON.parse(result);
                    alert(objjson[0]['message'])
                    $('#IDZalo').val(objjson[0]['error'])
                    //var option = {
                    //    url: '/Zalo/Index',
                    //    data: JSON.stringify({ id: 1 }),
                    //    method: 'post',
                    //    dataType: 'json',
                    //    contentType: 'application/json;charset=utf-8'
                    //};
                    //$.ajax(option).success();
                });
}