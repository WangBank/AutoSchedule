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

layui.use(['form', 'layedit', 'laydate'], function () {
    var form = layui.form
        , layer = layui.layer;
    form.on('submit(enSure)', function (data) {
        var dataDetail = JSON.stringify(data.field);
        console.log(dataDetail);
        $.ajax({
            url: "/TaskPlan/TaskPlanDetailAdd",
            async: true,
            type: "POST",
            dataType: "json",
            contentType: "application/json;charset=UTF-8",
            data: dataDetail,
            success: function (dataResult) {
                if (dataResult.code != "0") {
                    alert(dataResult.msg);
                }
                var index = parent.layer.getFrameIndex(window.name);
                setTimeout(function () { parent.layer.close(index) }, 1000);
                setTimeout(function () { parent.location.reload() }, 100);
            }
        });

    });
});