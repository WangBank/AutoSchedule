function GetDsList() {
    $.get("/DataSource/DataSourceResult", function (data) {
        if (data.code == 0) {
            var table = data.data;
            for (var i = 0; i < table.length; i++) {
                var item = table[i];
                $("#selectOrg").append("<option value=\"" + item.GUID + "\" selected=\"\">" + item.Name + "</option>");
            }
            renderForm();
        }
    }, "json");
}
function renderForm() {
    layui.use('form', function () {
        var form = layui.form;
        form.render();
    });
}

function formClose() {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);//关闭当前页
}

function beginPlan() {
    $.get("/Home/BeginTaskPlan?guid=", function (dataResult) {
        $('.alert').html(dataResult.msg).addClass('alert-success').show().delay(2000).fadeOut();
        table.reload('taskDtId');
    }, "json");

}

function stopPlan() {
    $.get("/Home/StopTaskPlan", function (dataResult) {
        $('.alert').html(dataResult.msg).addClass('alert-success').show().delay(2000).fadeOut();
        table.reload('taskDtId');
    }, "json");
}

function renderDone(res) {
   // console.log(res);
}


