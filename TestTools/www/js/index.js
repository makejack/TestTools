var table, formSelects;

$(document).ready(function () {
    $(".layui-nav-item").click(function (evt) {
        var nav = $(".layui-show");
        var navindex = $(".main div").index(nav);
        var index = $(".layui-nav li").index(this);
        if (navindex != index) {
            nav.removeClass("layui-show");
            $(".main").children().eq(index).addClass("layui-show");
        }
    })

    $(".move-group").click(function () {
        var effect = $(".top-move-effect");
        if (effect.length > 0) {
            $(".layui-show .left-container").removeClass("top-move-effect");
        } else {

            $(".layui-show .left-container").addClass("top-move-effect");
        }
    })

    $("#btn_issue").click(function () {
        layer.open({
            type: 1,
            area: "800px",
            title: "发行",
            closeBtn: 2,
            move: false,
            offset: "100px",
            content: $("#issue"),
            success: function (layero, index) {
                table.reload("vicetable", {
                    data: [{
                        "number": "afdasdf",
                        "time": "adf",
                        "partition": "aaaa"
                      }]
                });
            }
        });
    })

    $("#btn_batch").click(function () {
        layer.open({
            type: 1,
            area: "800px",
            title: "批量",
            closeBtn: 2,
            move: false,
            offset: "100px",
            content: $("#batch"),

        });
    })

    $("#btn_personnel_issue").click(function () {
        layer.open({
            type: 3,
        })
    })

})

layui.config({
    base: "plugins/formSelects/",
}).extend({
    formSelects: "formSelects-v4",
});

layui.use(['layer', 'laypage', 'element', 'laydate', 'formSelects'], function () {
    var layer = layui.layer
    laypage = layui.laypage,
        element = layui.element,
        laydate = layui.laydate,
        formSelects = layui.formSelects;


    laydate.render({
        elem: "#hostdate",
        value: new Date(),
    });

    laydate.render({
        elem: "#batchdate",
        value: new Date(),
    });

});

layui.use('table', function () {
    table = layui.table;

    table.render({
        elem: '#table_device_confirm',
        page: {
            layout: ['prev', 'page', 'next', 'count']
        },
        limit: 30,
        cellMinWidth: 100,
        height: 'full-70',
        cols: [[
            {
                title: '#',
                type: 'checkbox',
                fixed: true
            },
            {
                title: '主机编号'
            },
            {
                title: '进出口'
            },
            {
                title: '道闸编号'
            },
            {
                title: '开门模式'
            },
            {
                title: '车场分区'
            },
            {
                title: '防潜回'
            },
            {
                title: '车位限制'
            },
            {
                title: '读卡距离'
            },
            {
                title: '读卡延迟'
            },
            {
                title: '车牌识别'
            },
            {
                title: '无线编号'
            },
            {
                title: '频率偏移'
            },
            {
                title: '语言种类'
            },
            {
                title: '模糊查询位数',
                width: 120
            }
                ]]
    });

    table.render({
        elem: '#table_client',
        page: {
            layout: ['prev', 'page', 'next', 'count']
        },
        limit: 30,
        height: 'full-70',
        cols: [[
            {
                title: '#',
                width: 100
            },
            {
                title: '客户名称'
            },
            {
                title: '客户编号',
                width: 100
            },
            {
                title: '说明'
            }
                ]]
    });

    table.on("tool(vicetable)", function (obj) {
        var data = obj.data;
        if (obj.event === "edit") {
            layer.msg("edit");
        } else if (obj.event === "del") {
            obj.del();
        }
    });

});

layui.use('form', function () {
    var form = layui.form;

    form.verify({
        pwd: function (value) {
            if (value.length < 6) {
                return "密码长度不正确（6 位数字）";
            }
        },
        client: function (value) {
            if (value.length < 4) {
                return "客户编号长度不正确（4 位数字）";
            }
        },
        len: function (value) {
            var defaultRet = /^[Ff]{8}$/;
            var numberRet = /^[0-9]{8}$/;
            if (value.length < 8) {
                return "密码长度不正确（8 位数字）";
            } else if (!defaultRet.test(value)) {
                if (!numberRet.test(value)) {
                    return "密码为数字（8位）或F";
                }
            }
        }
    })

    form.on("submit(btn_distance_enter)", function () {
        layer.msg("button");
        return false;
    })

    form.on("checkbox(distance_default_old_pwd)", function (data) {
        DefaultEvent(data.elem.checked, $("#distance_old_pwd"));
    })

    form.on("checkbox(distance_default_pwd)", function (data) {
        DefaultEvent(data.elem.checked, $("#distance_pwd"));
    })

    form.on("checkbox(distance_confirm_default_pwd)", function (data) {
        DefaultEvent(data.elem.checked, $("#distance_confirm_pwd"));
    })

    form.on("checkbox(distance_default_number)", function (data) {
        if (data.elem.checked) {
            $("#distance_client_number").val("9887");
        } else {
            $("#distance_client_number").val("");
        }
        $("#distance_client_number").attr("disabled", data.elem.checked);
    })

    function DefaultEvent(checked, elem) {
        if (checked) {
            elem.val("766554");
        } else {
            elem.val("");
        }
        elem.attr("disabled", checked);
    }

    form.on("radio(distance)", function (data) {
        var disabled = !(data.value == "0");

        if (!$("#distance_default_old_pwd").is(":checked")) {
            $("#distance_old_pwd").attr("disabled", disabled);
        }
        $("#distance_default_old_pwd").attr("disabled", disabled);
        if (!$("#distance_default_number").is(":checked")) {
            $("#distance_client_number").attr("disabled", disabled);
        }
        $("#distance_default_number").attr("disabled", disabled);
    })

    form.on("submit(btn_ic_enter)", function (data) {

        return false;
    })

    form.on("checkbox(ic_default_old_pwd)", function (data) {
        DefaultEvent2(data.elem.checked, $("#ic_old_pwd"));
    })

    form.on("checkbox(ic_default_pwd)", function (data) {
        DefaultEvent2(data.elem.checked, $("#ic_pwd"));
    })

    form.on("checkbox(ic_default_confirm_pwd)", function (data) {
        DefaultEvent2(data.elem.checked, $("#ic_confirm_pwd"));
    })

    function DefaultEvent2(checked, elem) {
        if (checked) {
            elem.val("FFFFFFFF");
        } else {
            elem.val("");
        }
        elem.attr("disabled", checked);
    }

    form.on("radio(ic)", function (data) {
        var disabled = !(data.value == "0");

        $("#ic_default_old_pwd").attr("disabled", disabled);
        if (!$("#ic_default_old_pwd").is(":checked")) {
            $("#ic_old_pwd").attr("disabled", disabled);
        }
    })

    form.on("select(hosttype)", function (data) {
        if (data.value == "0") {
            $("#vice_group").css("display", "none");
            $("#partition_group").css("display", "block");
        } else {
            $("#vice_group").css("display", "block");
            $("#partition_group").css("display", "none");
        }
    })

    form.on("submit(btnaddvice)", function (data) {
        var strindex = $("#hosttype").val();
        layer.open({
            type: 1,
            area: "800px",
            title: "发行",
            closeBtn: 2,
            move: false,
            content: $("#vice"),
            offset: "150px",
            btn: ["确认"],
            yes: function (index, layero) {
                layer.msg(len);
                return false;
            },
            cancel: function () {
                form.val("viceform", {
                    "licenseplate": "",
                });
                formSelects.value("select2", []);
                formSelects.value("select3", []);
            },
            success: function () {
                if (strindex == "1") {
                    $("#licenseplate_group").css("display", "none");
                    $("#vicecard_group").css("display", "block");
                } else {
                    $("#licenseplate_group").css("display", "block");
                    $("#vicecard_group").css("display", "none");
                }
                laydate.render({
                    elem: "#vicedate",
                    value: new Date(),
                })
            }
        })
        return false;
    })

    form.on("submit(btnissueenter)", function (data) {
        console.log(data.field);
        return false;
    })

    form.on("submit(btnbatchenter)", function (data) {
        layer.msg('加载中', {
            icon: 16,
            shade: 0.01,
            time: 0,
        });
        return false;
    })
})