/// <reference path="../typings/index.d.ts" />

var BUTTON_DISABLED_CLASS = 'layui-btn-disabled',
    BUTTON_DANGER_CLASS = 'layui-btn-danger',
    ROW_SELECT_CLASS = 'layui-bg-select',
    DISABLED_PROPERTY = 'disabled',
    DEFAULT_DISTANCE_PASSWORD = '766554',
    DEFAULT_IC_DEVICE_PASSWORD = 'FFFFFFFF';

var form,
    laypage,
    table,
    formSelects;

var loadingIndex,
    windowsIndex,
    otherWindowsIndex,
    rowIndex = -1;

layui.config({
    base: "plugins/formSelects/",
}).extend({
    formSelects: "formSelects-v4",
});


layui.use(['layer', 'form', 'table', 'laypage', 'laydate', 'formSelects'], function () {
    form = layui.form;
    formSelects = layui.formSelects;
    laypage = layui.laypage;
    table = layui.table;
    var layer = layui.layer,
        laydate = layui.laydate;

    form.verify({
        notnull: function (value, item) {
            if (value.length == 0) {
                return "内容不能为空。";
            }
        },
        len4: function (value, item) {
            if (value.length < 4) {
                return "长度不正确（4 位数字）。"
            }
        },
        len6: function (value, item) {
            var elem = $("#" + item.id);
            if (!elem.prop(DISABLED_PROPERTY) || elem.prop("display")) {
                if (value.length < 6) {
                    return "长度不正确（6位数字）。";
                } else {
                    var regex = /^[0-9]{6}$/
                    if (!regex.test(value)) {
                        return "只能填写数字。";
                    }
                }
            }
        },
        len6number: function (value, item) {
            if (value.length < 6) {
                return "长度不正确（6位数字）。";
            }
        },
        len8: function (value, item) {
            var elem = $("#" + item.id);
            if (!elem.prop(DISABLED_PROPERTY)) {
                if (value.length < 8) {
                    return "长度不正确（8 位数字）。"
                } else {
                    var regex = /^[0-9]{8}$/;
                    if (!regex.test(value)) {
                        return "只能填写数字。";
                    }
                }
            }
        },
        selectlen: function (value, item) {
            var count = value ? value.split(",").length : 0;
            if (count == 0) {
                return "请选择挂失的卡片编号";
            }
        }
    })

    /* 设置 
     *  **************************************************************************************************************
     */

    $("#btn_setting").click(function () {
        layer.open({
            title: "设置",
            type: 1,
            area: '400px',
            move: false,
            closeBtn: 2,
            content: $("#setting"),
        });
    })

    form.on("select(connectionmode)", function (data) {
        var btns = $("button#btn_Device");
        var connModel = data.value == "0";
        HostAutoConnectionDeviced(connModel);
        btns.each(function () {
            var elem = $(this);
            if (!elem.hasClass("layui-btn-danger")) {
                var target = elem.attr("target");
                $("#" + target).prop(DISABLED_PROPERTY, connModel);
            }
            if (connModel) {
                elem.addClass(BUTTON_DISABLED_CLASS);
            } else {
                elem.removeClass(BUTTON_DISABLED_CLASS);
            }
            elem.prop(DISABLED_PROPERTY, connModel);
        });

        form.render("select", "settingform");
    })

    form.on("submit(btn_Device)", function () {
        var btn = $(this);
        var target = btn.attr("target");
        var select = $("#" + target);
        var name = select.attr("name");
        HostOpenAndCloseSerialDevice(name, select.val(), function (json) {
            var objJson = JSON.parse(json);
            var open = objJson["IsOpen"];
            SettingControlChange(btn, select, open);
        });
        return false;
    })

    /* 设置 
     *  **************************************************************************************************************
     */

    /* 表格 
     *  **************************************************************************************************************
     */

    $("#card_manager_table_tbody").on("click", "tr", function () {
        var row = $(this);
        if (!row.hasClass(ROW_SELECT_CLASS)) {
            var elem = $("#card_manager_table_tbody tr.layui-bg-select");
            if (elem) {
                elem.removeClass(ROW_SELECT_CLASS);
            };
            var row = $(this).addClass(ROW_SELECT_CLASS);
            rowIndex = row.index();

            if (!$('#btn_refresh').prop(DISABLED_PROPERTY)) {
                var json = HostPostCardInfo(rowIndex);
                var objJson = JSON.parse(json);
                var btnIssue = $('#btn_issue');
                var btnPersonnelIssue = $('#btn_personnel_issue');
                if (objJson['CardType'] > 2) {
                    IssueControlDisabled(btnIssue);
                    IssueControlDisabled(btnPersonnelIssue);
                } else {
                    IssueControlEnabled(btnIssue);
                    IssueControlEnabled(btnPersonnelIssue);
                }
            }
        }
    })

    function IssueControlEnabled(btn) {
        if (btn.prop(DISABLED_PROPERTY)) {
            btn.prop(DISABLED_PROPERTY, false);
            btn.removeClass(BUTTON_DISABLED_CLASS);
        }
    }

    function IssueControlDisabled(btn) {
        if (!btn.prop(DISABLED_PROPERTY)) {
            btn.prop(DISABLED_PROPERTY, true);
            btn.addClass(BUTTON_DISABLED_CLASS);
        }
    }

    $('#card_manager_table_tbody').contextmenu(function (e) {
        var item = [{
                title: '删除行',
                icon: 'fa fa-close',
                fn: function () {
                    layer.confirm('确认是否删除当前选择的行数据。', {
                        btn: ['删除', '不删除'],
                        icon: 2,
                        closeBtn: 2,
                    }, function (index) {
                        var ret = HostPostDelRowData(rowIndex);
                        if (ret) {
                            $('#card_manager_table_tbody').children()[rowIndex].remove();
                            rowIndex = -1;
                            IssueControlDisabled($('#btn_issue'));
                            IssueControlDisabled($('#btn_personnel_issue'));
                        } else {
                            layer.msg('删除失败，数据未发行（注册）。', {
                                icon: 0,
                                anim: 6,
                            });
                        }
                        layer.close(index);
                    });
                },
                disabled: rowIndex == -1,
            },
            {
                title: '删除全部数据',
                icon: 'fa fa-trash',
                fn: function () {
                    layer.confirm('确认是否删除全部数据。', {
                        btn: ['删除', '不删除'],
                        icon: 2,
                        closeBtn: 2,
                    }, function (index) {
                        var ret = HostPostDelAllData();
                        if (ret) {
                            $('#card_manager_table_tbody').html('');
                            IssueControlDisabled($('#btn_issue'));
                            IssueControlDisabled($('#btn_personnel_issue'));
                        }
                        layer.close(index);
                    });
                },
            }
        ];
        basicContext.show(item, e);
    })

    $("#download_table").on('click', 'tr', function () {
        var elem = $(this);
        if (!elem.hasClass(ROW_SELECT_CLASS)) {
            var selectElem = $('#download_table tr.layui-bg-select');
            if (selectElem) {
                selectElem.removeClass(ROW_SELECT_CLASS);
            }

            elem.addClass(ROW_SELECT_CLASS);

            DownloadControlEnabled(false);
        }
    })

    /* 表格 
     *  **************************************************************************************************************
     */

    /* 卡片管理
     *  **************************************************************************************************************
     */

    $("#btn_refresh").click(function () {
        $("#card_manager_table_tbody").html("");
        $('#btn_issue').prop(DISABLED_PROPERTY, true).addClass(BUTTON_DISABLED_CLASS);
        $('#btn_personnel_issue').prop(DISABLED_PROPERTY, true).addClass(BUTTON_DISABLED_CLASS);
        rowIndex = -1;

        var ret = HostRefreshOperation();
        if (ret) {
            LoadingMsg('Loading···');
        }
    })

    $("#btn_issue").click(function () {
        var json = HostPostCardInfo(rowIndex);
        var objJson = JSON.parse(json);
        form.val('issueform', {
            number: objJson["CardNumber"],
            date: formatDate(new Date(objJson["CardTime"])),
            distance: objJson["CardDistance"],
            limit: objJson["ParkingRestrictions"],
            type: objJson['CardType'],
        });
        ViewDisplayViceCardInfo(objJson.ViceCardInfos);

        windowsIndex = layer.open({
            title: "定距卡发行（注册）",
            type: 1,
            area: '740px',
            move: false,
            closeBtn: 2,
            offset: '100px',
            resize: false,
            content: $('#issue'),
            success: function () {
                formSelects.value('select1', PartitionToArray(objJson['CardPartition']));
                IssueTypeChange();
            },
            end: function () {
                /*防止分区出现高度变小的bug*/
                $('#partition_group').show();
            },
        });
    })

    function ViewDisplayViceCardInfo(json) {
        // var content = '';
        // $.each(json, function (index, item) {
        //     var time = formatDate(new Date(item.CardTime));
        //     var partition = PartitionToStr(item.CardPartition);
        //     content += "<tr><td>" + item.CardNumber + "</td><td>" + time + "</td><td class='table-overflow'>" + partition + "</td><td><a class='layui-btn layui-btn-xs' lay-submit='' lay-filter='viceedit'>编辑</a><a class='layui-btn layui-btn-danger layui-btn-xs' lay-submit='' lay-filter='vicedel'>删除</a></td></tr>";
        // });
        // $('#vice_card_table_tbody').html(content);
        table.reload('vicetable', {
            data: json,
        });
        if (json && json.length >= 4) {
            $('.vice-add-group').prop('display', true);
        }
    }

    form.on('select(issuetype)', function () {
        IssueTypeChange();
    })

    function IssueTypeChange() {
        var currType = parseInt($('#issue_type').val());
        var cardType = HostPostHostType(rowIndex);
        if (currType != cardType) {
            var count = HostPostViceCardCount(rowIndex);
            if (count > 0) {
                layer.confirm('定距卡类型修改，是否删除所有相关的副卡信息。', {
                    btn: ['删除', '不删除'],
                    closeBtn: 2,
                    move: false,
                    icon: 3,
                }, function (index) {
                    HostPostDelViceCardInfos(rowIndex);
                    table.reload('vicetable', {
                        data: null,
                    });
                    layer.close(index);
                }, function (index) {
                    $('#issue_type').val(cardType);
                    IssueTypeChange();
                    layer.close(index);
                });
            }
        }

        if (currType == 0) {
            $('#issue_limit').prop(DISABLED_PROPERTY, true);
            $('#issue_date').prop(DISABLED_PROPERTY, false);
            $('#partition_group').show();
            $('#vicegroup').hide();
        } else {
            $('#issue_limit').prop(DISABLED_PROPERTY, false);
            $('#issue_date').prop(DISABLED_PROPERTY, true);
            $('#partition_group').hide();
            $('#vicegroup').show();
        }
        form.render(null, 'issueform');
    }

    table.render({
        elem: '#vicetable',
        id: 'vicetable',
        cols: [
            [{
                    field: 'CardLock',
                    width: 49,
                    unresize: true,
                    align: 'center',
                    templet: function (data) {
                        if (data.CardLock == 0) {
                            return "<div class='table-cell-center'><i class='fa fa-check'></i></div>";
                        } else {
                            return "<div class='table-cell-center'><i class='fa fa-close'></i></div>";
                        }
                    }
                },
                {
                    field: 'CardNumber',
                    title: '卡号或车牌号',
                    width: 130,
                    unresize: true
                },
                {
                    field: 'CardTime',
                    title: '时间',
                    width: 120,
                    unresize: true,
                    templet: function (data) {
                        return formatDate(new Date(data.CardTime));
                    },
                },
                {
                    field: 'CardPartition',
                    title: '车场分区',
                    width: 300,
                    unresize: true,
                    templet: function (data) {
                        return PartitionToStr(data.CardPartition);
                    }
                },
                {
                    title: '操作',
                    width: 115,
                    unresize: true,
                    toolbar: '#barvicetable',
                },
            ]
        ]
    })

    table.on("tool(vicetable)", function (data) {
        if (data.event == 'edit') {
            $('#licenseplategroup').show();
            $('#vicecardgroup').hide();
            var type = $('#issue_type').val();
            $('#vice_number').prop(DISABLED_PROPERTY, (type == 1));

            var index = data.tr.index();
            var json = HostPostViceCardInfo(rowIndex, index);
            var objJson = JSON.parse(json);
            form.val('viceform', {
                index: index,
                number: objJson['CardNumber'],
                date: formatDate(new Date(objJson['CardTime'])),
            });

            otherWindowsIndex = layer.open({
                title: "编辑副卡",
                type: 1,
                area: '740px',
                move: false,
                closeBtn: 2,
                offset: '100px',
                resize: false,
                content: $('#vice'),
                success: function () {
                    formSelects.value('select3', PartitionToArray(objJson['CardPartition']));
                }
            });
        } else if (data.event == 'del') {
            layer.confirm('确认是否删除副卡信息。', {
                btn: ['删除', '不删除'],
                icon: 0,
                closeBtn: 2,
            }, function (index) {
                var row = data.tr.index();
                HostPostDelViceCardInfo(rowIndex, row);
                data.del();
                $('.vice-add-group').prop('display', false);
                layer.close(index);
            });
        }
    })

    form.on("submit(viceadd)", function () {
        var value = $('#issue_type').val();

        otherWindowsIndex = layer.open({
            title: "添加副卡",
            type: 1,
            area: '740px',
            move: false,
            closeBtn: 2,
            offset: '100px',
            content: $('#vice'),
            success: function () {
                if (value == 1) {
                    $('#licenseplategroup').hide();
                    $('#vicecardgroup').show();

                    var json = HostPostViceCardList(rowIndex);
                    if (json) {
                        var objJson = JSON.parse(json);
                        var array = new Array();
                        $.each(objJson, function (index, item) {
                            array[index] = {
                                'name': item.CardNumber,
                                'value': item.CardNumber
                            };
                        });
                        formSelects.data('select2', 'local', {
                            arr: array,
                        });
                    }
                } else {
                    $('#licenseplategroup').show();
                    $('#vicecardgroup').hide();
                }
                form.val('viceform', {
                    index: -1,
                    number: '',
                    date: formatDate(new Date()),
                })
                formSelects.value('select3', []);
                $('#vice_number').prop(DISABLED_PROPERTY, false);
            },
            end: function () {
                $('#vicecardgroup').show();
            }
        });

        return false;
    })

    form.on("submit(btn_vice_enter)", function (data) {
        // var number = $('#vice_number').val();
        var index = parseInt(data.field.index);
        var number = data.field.number;
        var type = parseInt($('#issue_type').val());
        if (type == 1) {
            if (index < 0) {
                number = formSelects.value('select2', 'valStr');
                if (number.length == 0) {
                    layer.tips('请选择副卡。', '.xm-select-parent[fs_id=select2]', {
                        tips: 1,
                    });
                    return false;
                }
            }
        } else {
            if (number.length == 0) {
                layer.tips('车牌号码不能为空。', '#vice_number', {
                    tips: 1,
                });
                return false;
            }
        }

        // var time = new Date($('#vice_date').val());
        var time = new Date(data.field.date);
        var partitions = formSelects.value('select3', 'val');
        var partition = PartitionToInt(partitions);

        var json = HostPostUpdateViceCardInfo(rowIndex, index, number, type, time, partition);
        if (json) {
            var objJson = JSON.parse(json);
            ViewDisplayViceCardInfo(objJson);
            layer.close(otherWindowsIndex);
        }
        return false;
    })

    form.on("submit(btn_issue_enter)", function (data) {
        //var number = $('#issue_number').val();
        // var time = new Date($('#issue_date').val());
        var time = new Date(data.field.date);
        // var distance = parseInt($('#issue_distance').val());
        var distance = parseInt(data.field.distance);
        // var limit = parseInt($('#issue_limit').val());
        var limit = parseInt(data.field.limit);
        // var type = parseInt($('#issue_type').val());
        var type = parseInt(data.field.type);
        var partitions = formSelects.value('select1', 'val');
        // var partition = PartitionToInt(partitions);
        var partition = PartitionToInt(partitions);

        var ret = HostPostIssue(rowIndex, time, distance, limit, type, partition);
        if (ret) {
            LoadingMsg('发行中···');
        }
        return false;
    })

    $("#btn_batch").click(function () {
        var json = HostPortBatchParameter();
        var objJson = JSON.parse(json);
        form.val('batchform', {
            date: formatDate(new Date(objJson['CardTime'])),
            distance: objJson['CardDistance'],
        });

        windowsIndex = layer.open({
            title: "定距卡批量发行（注册）",
            type: 1,
            area: '740px',
            move: false,
            closeBtn: 2,
            offset: '100px',
            resize: false,
            content: $('#batch'),
            success: function () {
                var partitions = PartitionToArray(objJson['CardPartition']);
                formSelects.value('select4', partitions);
            },
        });
    })

    form.on("submit(btn_batch_enter)", function (data) {
        // var time = $('#batch_date').val();
        var time = new Date(data.field.date);
        // var distance = $('#batch_distance').val();
        var distance = parseInt(data.field.distance);
        var partitions = formSelects.value('select4', 'val');
        var partition = PartitionToInt(partitions);

        HostPostBatch(time, distance, partition);

        LoadingMsg('批量发行中···');
        return false;
    })

    $("#btn_personnel_issue").click(function () {
        var json = HostPostCardInfo(rowIndex);
        var objJson = JSON.parse(json);
        var time = formatDate(new Date(objJson['CardTime']));

        form.val('personnelform', {
            number: objJson['CardNumber'],
            index: rowIndex,
            date: time,
        });
        $('#perosnnel_number_group').show();

        windowsIndex = layer.open({
            title: "人员通道定距卡发行（注册）",
            type: 1,
            area: '740px',
            move: false,
            closeBtn: 2,
            offset: '100px',
            resize: false,
            content: $('#personnel'),
            success: function () {
                var parations = PartitionToArray(objJson['CardPartition']);
                formSelects.value('select5', parations);
            },
        });
    })

    $("#btn_personnel_batch").click(function () {
        var json = HostPortBatchParameter();
        var objJson = JSON.parse(json);
        form.val('personnelform', {
            number: '',
            index: -1,
            date: formatDate(new Date(objJson['CardTime'])),
        });
        $('#perosnnel_number_group').hide();

        windowsIndex = layer.open({
            title: "人员通定距卡道批量发行（注册）",
            type: 1,
            area: '740px',
            move: false,
            closeBtn: 2,
            offset: '100px',
            resize: false,
            content: $('#personnel'),
            success: function () {
                var partitions = PartitionToArray(objJson['CardPartition']);
                formSelects.value('select5', partitions)
            },
        })
    })

    form.on("submit(btn_personnel_enter)", function (data) {
        //var time = new Date($('#personnel_date').val());
        var time = new Date(data.field.date);
        var index = parseInt(data.field.index);
        var partitions = formSelects.value('select5', 'val');
        var partition = PartitionToInt(partitions);

        var msg = "发行中···";
        var ret = true;
        if (index > -1) {
            ret = HostPostPersonnelIssue(rowIndex, time, partition);
        } else {
            HostPostPersonnelBatch(time, 0, partition);
            msg = "批量发行中···";
        }
        if (ret) {
            LoadingMsg(msg);
        }
        return false;
    })

    $('#btn_lose').click(function () {
        windowsIndex = layer.open({
            title: '挂失',
            type: 1,
            closeBtn: 2,
            move: false,
            area: '500px',
            offset: '100px',
            resize: false,
            content: $('#lose'),
            success: function () {
                var json = HostPostLoseInfos(0);
                var objJson = JSON.parse(json);
                var array = new Array();
                $.each(objJson, function (index, item) {
                    array[index] = {
                        'name': item.CardNumber,
                        'value': item.CardNumber,
                    };
                });
                formSelects.data('select6', 'local', {
                    arr: array,
                });
            }
        });
    })

    form.on('submit(btn_distance_lose)', function () {
        var values = formSelects.value('select6', 'val');
        var ret = HostPostDistanceLose(values);
        if (ret) {
            LoadingMsg("挂失中···");
        }
        return false;
    })

    form.on('submit(btn_personnel_lose)', function () {
        var values = formSelects.value('select6', 'val');
        var ret = HostPostPersonnelLose(values);
        if (ret) {
            LoadingMsg("挂失中···");
        }
        return false;
    })

    $('#btn_recovery').click(function () {
        windowsIndex = layer.open({
            title: '解除挂失',
            type: 1,
            closeBtn: 2,
            move: false,
            area: '500px',
            offset: '100px',
            resize: false,
            content: $('#recovery'),
            success: function () {
                var json = HostPostLoseInfos(1);
                var objJson = JSON.parse(json);
                var array = new Array();
                $.each(objJson, function (index, item) {
                    array[index] = {
                        'name': item.CardNumber,
                        'value': item.CardNumber,
                    };
                });
                formSelects.data('select7', 'local', {
                    arr: array,
                });
            }
        });
    })

    form.on('submit(btn_personnel_recovery)', function () {
        var values = formSelects.value('select7', 'val');
        var ret = HostPostPersonnelRecovery(values);
        if (ret) {
            LoadingMsg("解挂中···");
        }
        return false;
    })

    /* 卡片管理
     *  **************************************************************************************************************
     */

    /* 加密管理
     *  **************************************************************************************************************
     */

    $(".site-nav").click(function () {
        // var maxheight = $(window).height();
        var elem = $("#passwordgroup");
        var top = elem.offset().top;
        if (top >= 0) {
            elem.velocity({
                translateY: "-100%",
            }, 200);
        } else {
            elem.velocity({
                translateY: 0,
            }, 200);
        }
    })

    form.on("checkbox(distance_default_old_pwd)", function (data) {
        if (data.elem.checked) {
            $("#distance_old_pwd").val(DEFAULT_DISTANCE_PASSWORD);
            $("#distance_old_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#distance_old_pwd").val("");
            $("#distance_old_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("checkbox(distance_default_pwd)", function (data) {
        if (data.elem.checked) {
            $("#distance_pwd").val(DEFAULT_DISTANCE_PASSWORD);
            $("#distance_confirm_pwd").val(DEFAULT_DISTANCE_PASSWORD);
            $("#distance_pwd").attr(DISABLED_PROPERTY, true);
            $("#distance_confirm_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#distance_pwd").val("");
            $("#distance_confirm_pwd").val("");
            $("#distance_pwd").removeAttr(DISABLED_PROPERTY);
            $("#distance_confirm_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("checkbox(distance_default_number)", function (data) {
        if (data.elem.checked) {
            $("#distance_client_number").val("9887");
            $("#distance_client_number").attr(DISABLED_PROPERTY, true);
        } else {
            $("#distance_client_number").val("");
            $("#distance_client_number").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("radio(distance)", function (data) {
        var elem = $("#distanceoldpwdgroup");
        if (data.value == "0") {
            elem.show();
            elem.velocity({
                translateX: 0,
                opacity: 1,
            }, 200, function () {
                if (!$('#distance_default_old_pwd').prop('checked')) {
                    $('#distance_old_pwd').prop(DISABLED_PROPERTY, false);
                }
            });
        } else {
            elem.velocity({
                translateX: "100%",
                opacity: 0,
            }, 200, function () {
                $('#distance_old_pwd').prop(DISABLED_PROPERTY, true);
                elem.hide();
            });
        }
    })

    form.on("submit(btn_distance_enter)", function (data) {
        var oldpwd = data.field.oldpwd; // $('#distance_old_pwd').val();
        var pwd = data.field.pwd; // $('#distance_pwd').val();
        var confirmpwd = data.field.confirmpwd; // $('#distance_confirm_pwd').val();
        var clientNumber = data.field.clientnumber; // $('#distance_client_number').val();
        if (pwd != confirmpwd) {
            $('#distance_pwd').addClass("layui-form-danger").focus();
            $('#distance_confirm_pwd').addClass("layui-form-danger");
            layer.msg("新密码与确认密码不一致。", {
                icon: 2,
                anim: 6,
            });
            return false;
        }

        var value = data.field.distance;
        if (value == 0) {
            HostPostDistanceCardEncrypt(oldpwd, pwd, clientNumber);
        } else {
            HostPostDistanceDeviceEncrypt(pwd, clientNumber);
        }
        EncryptControlEnabled(true);
        return false;
    })

    form.on("checkbox(ic_default_old_pwd)", function (data) {
        if (data.elem.checked) {
            $("#ic_old_pwd").val(DEFAULT_IC_DEVICE_PASSWORD);
            $("#ic_old_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#ic_old_pwd").val("");
            $("#ic_old_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("checkbox(ic_default_pwd)", function (data) {
        if (data.elem.checked) {
            $("#ic_pwd").val(DEFAULT_IC_DEVICE_PASSWORD);
            $("#ic_pwd").attr(DISABLED_PROPERTY, true);
            $("#ic_confirm_pwd").val(DEFAULT_IC_DEVICE_PASSWORD);
            $("#ic_confirm_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#ic_pwd").val("");
            $("#ic_pwd").removeAttr(DISABLED_PROPERTY);
            $("#ic_confirm_pwd").val("");
            $("#ic_confirm_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("radio(ic)", function (data) {
        var elem = $("#icoldpwdgroup");
        if (data.value == "0") {
            elem.show();
            elem.velocity({
                translateX: 0,
                opacity: 1,
            }, 200, function () {
                if (!$('#ic_default_old_pwd').prop('checked')) {
                    $('#ic_old_pwd').prop(DISABLED_PROPERTY, false);
                }
            });
        } else {
            elem.velocity({
                translateX: "100%",
                opacity: 0,
            }, 200, function () {
                $('#ic_old_pwd').prop(DISABLED_PROPERTY, true);
                elem.hide();
            });
        }
    })

    form.on("submit(btn_ic_enter)", function (data) {
        var oldpwd = data.field.oldpwd; // $('#ic_old_pwd').val();
        var pwd = data.field.pwd; // $('#ic_pwd').val();
        var confirmpwd = data.field.confirmpwd; // $('#ic_confirm_pwd').val();
        if (pwd != confirmpwd) {
            $('#ic_pwd').addClass("layui-form-danger").focus();
            $('#ic_confirm_pwd').addClass("layui-form-danger");
            layer.msg("新密码与确认密码不一致。", {
                icon: 2,
                anim: 6,
            });
            return false;
        }

        var value = data.field.ic;
        if (value == 0) {
            var btn = $('#btn_ic_encrypt');
            if (btn.hasClass(BUTTON_DANGER_CLASS)) {
                HostPostStopIcCardEncrypt();
                EncryptControlEnabled(false);
                return false;
            } else {
                HostPostStartIcCardEncrypt(oldpwd, pwd);
                btn.addClass(BUTTON_DANGER_CLASS);
                btn.text("停止");
            }
        } else {
            HostPostIcDeviceEncrypt(pwd);
        }
        EncryptControlEnabled(true);
        return false;
    })

    $('#btn_clear_pwd_list').click(function () {
        $('#password_news').html('');
    })

    /* 加密管理
     *  **************************************************************************************************************
     */

    /* 无线测试
     *  **************************************************************************************************************
     */

    form.on("submit(btn_Wireless_setup)", function () {
        var wirelessID = $("#wirelessID").val();
        var radioFrequency = $("#radiofrequency").val();
        HostPostWirelessSetting(wirelessID, radioFrequency);
        WirelessControlEnabled(true);
        return false;
    })

    $('#btn_wireless_search').click(function () {
        var btn = $(this);
        if (!btn.hasClass(BUTTON_DANGER_CLASS)) {
            HostPostWirelessSearch();
            btn.text("停止");
            btn.addClass(BUTTON_DANGER_CLASS);
            WirelessControlEnabled(true);
        } else {
            HostPostStopWirelessSearch();
            WirelessControlEnabled(false);
        }
    })

    $('#btn_wireless_test').click(function () {
        HostPostWirelessTest();
    })

    $('#btn_wireless_query').click(function () {
        HostPostWirelessQuery();
        WirelessControlEnabled(true);
    })

    $('#btn_refresh_ic').click(function () {
        var btn = $(this);
        if (!btn.hasClass(BUTTON_DANGER_CLASS)) {
            HostPostReadIcCard();
            btn.text("停止");
            btn.addClass(BUTTON_DANGER_CLASS);
            WirelessControlEnabled(true);
        } else {
            HostPostStopReadIcCard();
            WirelessControlEnabled(false);
        }
    })

    $('#btn_clear_wireless_list').click(function () {
        $('#wireless_news').html('');
    })

    /* 无线测试
     *  **************************************************************************************************************
     */


    /* 硬件配置
     *  **************************************************************************************************************
     */

    $('#btn_config_show').click(function () {
        var count = HostPostConfirmInfoCount();
        laypage.render({
            elem: 'configpager',
            limit: 30,
            count: count,
            layout: ['prev', 'page', 'next', 'count'],
            jump: function (obj, first) {
                var json = HostPostConfirmInfos(obj.curr);
                console.log(json);
                var objJson = JSON.parse(json);
                table.reload('configtable', {
                    data: objJson,
                });
            }
        })
    })

    /*车牌识别*/
    form.on("select(licenseplaterecognition)", function () {
        EnabledLicensePlateRecognition();
    })

    /*开闸模式*/
    form.on("select(openingmode)", function () {
        OpenModelChange();
    })

    $("#btn_config_add").click(function () {
        var number = HostPostHostNumber();
        form.val('configform', {
            HostNumber: number,
        });
        ShowConfigDialog("添加配置信息");
    })

    form.on("submit(btn_config_enter)", function (data) {
        var number = parseInt(data.field.HostNumber);
        if (number > 128) {
            layer.msg("主机超出范围最大128。", {
                anim: 6,
                icon: 2,
                closeBtn: 2,
            });
            return false;
        }
        var json = JSON.stringify(data.field);
        if (data.field.Did > 0) {
            HostPostUpdateConfirmInfo(json);
        } else {
            HostPostConfirmAdd(json);
        }
        $('#btn_config_show').trigger('click');
        CloseWindows();
        return false;
    })

    $("#btn_config_export").click(function () {
        var json = HostPostDrives();
        var objJson = JSON.parse(json);
        var content = "";
        $.each(objJson, function (index, item) {
            content += "<option value='" + item['_name'] + "'>" + item['_name'] + "</option>";
        });
        $('#Drives').html(content);
        form.render('select', 'exportform');

        windowsIndex = layer.open({
            title: "导出配置",
            type: 1,
            move: false,
            closeBtn: 2,
            offset: '100px',
            area: '740px',
            content: $("#export"),
        });
    })

    form.on("checkbox(enableddevicepwd)", function (data) {
        var row = $('#control_row');
        if (data.elem.checked) {
            var content = "<div class='layui-col-md5'><div class='layui-form-item'><label for='' class='layui-form-label'>新密码</label><div class='layui-input-block'><input type='text' class='layui-input' name='devicepwd' id='device_pwd' lay-filter='devicepwd' lay-verify=len6 required placeholder='6 位数字密码' maxlength='6'></div></div></div><div class='layui-col-md5'><div class='layui-form-item'><label for='' class='layui-form-label'>确认密码</label><div class='layui-input-block'><input type='text' class='layui-input' name='confirmdevicepwd' id='confirm_device_pwd' lay-filter='confirmdevicepwd' lay-verify=len6 required placeholder='6 位数字密码' maxlength='6'></div></div></div><div class='layui-col-md2'><div class='layui-form-item'><div class=''><input type='checkbox' title='默认密码' lay-skin='primary' lay-filter='defaultdevicepwd' id='default_device_pwd'></div></div></div>";
            row.html(content);

            row.velocity({
                translateX: 0,
                opacity: 1,
            }, 400);
        } else {
            row.velocity({
                translateX: '100%',
                opacity: 0,
            }, 400, function () {
                row.html('');
            });
        }
        form.render(null, "exportform");
    })

    form.on("checkbox(enabledhostpwd)", function (data) {
        var row = $('#host_row');
        if (data.elem.checked) {
            var content = "<div class='layui-row'><div class='layui-col-md10'><div class='layui-form-item'><label for='' class='layui-form-label'>旧密码</label><div class='layui-input-block'><input type='text' name='hostoldpwd' class='layui-input' id='host_old_pwd' lay-filter='hostoldpwd' lay-verify='len6' required placeholder='6 位数字密码' maxlength='6'></div></div></div><div class='layui-col-md2'><div class='layui-form-item'><div class=''><input type='checkbox' title='默认密码' lay-skin='primary' lay-filter='defaulthostoldpwd' id='default_host_old_pwd'></div></div></div></div><div class='layui-row'><div class='layui-col-md5'><div class='layui-form-item'><label for='' class='layui-form-label'>新密码</label><div class='layui-input-block'><input type='text' name='hostpwd' class='layui-input' id='host_pwd' lay-filter='hostpwd' lay-verify='len6' required placeholder='6 位数字密码' maxlength='6'></div></div></div><div class='layui-col-md5'><div class='layui-form-item'><label for='' class='layui-form-label'>确认密码</label><div class='layui-input-block'><input type='text' name='confirmhostpwd' class='layui-input' id='confirm_host_pwd' lay-filter='confirmhostpwd' lay-verify='len6' required placeholder='6 位数字密码'maxlength='6'></div></div></div><div class='layui-col-md2'><div class='layui-form-item'><div><input type='checkbox' name='' title='默认密码' lay-skin='primary' lay-filter='defaulthostpwd' id='default_host_pwd'></div></div></div></div>";

            row.html(content);
            row.velocity({
                translateX: 0,
                opacity: 1,
            }, 400);
        } else {
            row.velocity({
                translateX: '100%',
                opacity: 0,
            }, 400, function () {
                row.html('');
            });
        }
        form.render(null, "exportform");
    })

    form.on("checkbox(enabledicpwd)", function (data) {
        var row = $('#ic_row');
        if (data.elem.checked) {
            var content = "<div class='layui-col-md5'><div class='layui-form-item'><label for='' class='layui-form-label'>新密码</label><div class='layui-input-block'><input type='text' class='layui-input' name='icpwd' id='ic_device_pwd' lay-filter='icdevicepwd' lay-verify='len8' required placeholder='8 位数字密码' maxlength='8'></div></div></div><div class='layui-col-md5'><div class='layui-form-item'><label for='' class='layui-form-label'>确认密码</label><div class='layui-input-block'><input type='text' name='confirmicpwd' class='layui-input' id='confirm_ic_device_pwd' lay-filter='confirmicdevicepwd' lay-verify='len8' requiredplaceholder='8 位数字密码' maxlength='8'></div></div></div><div class='layui-col-md2'><div class='layui-form-item'><div><input type='checkbox' name='' title='默认密码' lay-skin='primary' lay-filter='defaulticdevicepwd' id='default_ic_device_pwd'></div></div></div>";
            row.html(content);
            row.velocity({
                translateX: 0,
                opacity: 1,
            }, 400);
        } else {
            row.velocity({
                translateX: '100%',
                opacity: 0,
            }, 400, function () {
                row.html('');
            });
        }
        form.render(null, "exportform");
    })

    form.on("checkbox(defaulthostoldpwd)", function (data) {
        if (data.elem.checked) {
            $("#host_old_pwd").val(DEFAULT_DISTANCE_PASSWORD);
            $("#host_old_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#host_old_pwd").val("");
            $("#host_old_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("checkbox(defaultdevicepwd)", function (data) {
        var disabled = data.elem.checked;
        var txt = "";
        $('#device_pwd').val(txt);
        $('#device_pwd').prop(DISABLED_PROPERTY, disabled);
        $('#confirm_device_pwd').val(txt);
        $('#confirm_device_pwd').prop(DISABLED_PROPERTY, disabled);
    })

    form.on("checkbox(defaulthostpwd)", function (data) {
        if (data.elem.checked) {
            $("#host_pwd").val(DEFAULT_DISTANCE_PASSWORD);
            $("#host_pwd").attr(DISABLED_PROPERTY, true);
            $("#confirm_host_pwd").val(DEFAULT_DISTANCE_PASSWORD);
            $("#confirm_host_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#host_pwd").val("");
            $("#host_pwd").removeAttr(DISABLED_PROPERTY);
            $("#confirm_host_pwd").val("");
            $("#confirm_host_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("checkbox(defaulticdevicepwd)", function (data) {
        if (data.elem.checked) {
            $("#ic_device_pwd").val(DEFAULT_IC_DEVICE_PASSWORD);
            $("#ic_device_pwd").attr(DISABLED_PROPERTY, true);
            $("#confirm_ic_device_pwd").val(DEFAULT_IC_DEVICE_PASSWORD);
            $("#confirm_ic_device_pwd").attr(DISABLED_PROPERTY, true);
        } else {
            $("#ic_device_pwd").val("");
            $("#ic_device_pwd").removeAttr(DISABLED_PROPERTY);
            $("#confirm_ic_device_pwd").val("");
            $("#confirm_ic_device_pwd").removeAttr(DISABLED_PROPERTY);
        }
    })

    form.on("submit(btn_export_enter)", function (data) {
        var drives = data.field.Drives;
        var checkStatus = JSON.stringify(table.checkStatus('configtable').data);
        var enabledControl = data.field.device;
        var controlPwd = '';
        var hostoldpwd = '';
        var hostpwd = '';
        var icpwd = '';

        if (data.field.device != undefined) {
            if (data.field.devicepwd != data.field.confirmdevicepwd) {
                $('#device_pwd').addClass('layui-form-danger').focus();
                $('#confirm_device_pwd').addClass('layui-form-danger');

                layer.msg('控制板新密码与确认密码不一致。', {
                    icon: 2,
                    anim: 6,
                });
                return false;
            }
            controlPwd = data.field.devicepwd;
        }
        if (data.field.host != undefined) {
            if (data.field.hostpwd != data.field.confirmhostpwd) {
                $('#host_pwd').addClass('layui-form-danger').focus();
                $('#confirm_host_pwd').addClass('layui-form-danger');

                layer.msg('主机系统口令新密码与确认密码不一致。', {
                    anim: 6,
                    icon: 2,
                });
                return false;
            }
            hostpwd = data.field.hostpwd;
            hostoldpwd = data.field.hostoldpwd;
        }
        if (data.field.ic != undefined) {
            if (data.field.icpwd != data.field.confirmicpwd) {
                $('#ic_device_pwd').addClass('layui-form-danger').focus();
                $('#confirm_ic_device_pwd').addClass('layui-form-danger');

                layer.msg('IC卡系统口令新密码与确认密码不一致。', {
                    anim: 6,
                    icon: 2,
                });
                return false;
            }
            icpwd = data.field.icpwd;
        }
        HostPostExport(drives, checkStatus, enabledControl, controlPwd, hostoldpwd, hostpwd, icpwd);
        layer.close(windowsIndex);
        return false;
    })

    table.render({
        elem: '#configtable',
        id: 'configtable',
        height: 'full-120',
        limit: 30,
        cols: [
            [{
                    type: 'checkbox',
                    fixed: 'left'
                },
                {
                    field: "Did",
                    width: 100,
                    title: '编号',
                    unresize: true
                },
                {
                    field: 'HostNumber',
                    width: 100,
                    title: '主机编号',
                    unresize: true
                },
                {
                    field: 'IOMouth',
                    width: 80,
                    title: '进出口',
                    unresize: true,
                    templet: function (e) {
                        return e.IOMouth == 0 ? "入口" : "出口";
                    }
                },
                {
                    field: 'BrakeNumber',
                    width: 100,
                    title: '道闸编号',
                    unresize: true
                },
                {
                    field: 'OpenModel',
                    width: 150,
                    title: '开门模式',
                    unresize: true,
                    templet: function (e) {
                        var opendoormodel = "继电器开闸";
                        switch (e.OpenModel) {
                            case 0:
                                opendoormodel = "畅泊：串口开闸";
                                break;
                            case 1:
                                opendoormodel = "畅泊：无线开闸";
                                break;
                            case 2:
                                opendoormodel = "学习遥控器";
                                break;
                        }
                        return opendoormodel;
                    }
                },
                {
                    field: 'Partition',
                    width: 100,
                    title: '车场分区',
                    unresize: true,
                    templet: function (e) {
                        var partition = "关闭";
                        if (e.Partition > 0) {
                            partition = "分区 " + e.Partition;
                        }
                        return partition;
                    }
                },
                {
                    field: 'SAPBF',
                    width: 80,
                    title: '防潜回',
                    unresize: true,
                    templet: function (e) {
                        return e.SAPBF == 0 ? "关闭" : "开启";
                    }
                },
                {
                    field: 'Detection',
                    width: 100,
                    title: '离开检测',
                    unresize: true,
                    templet: function (e) {
                        return e.Detection == 0 ? "关闭" : "开启";
                    }
                },
                {
                    field: 'CardReadDistance',
                    width: 100,
                    title: '读卡距离',
                    unresize: true,
                    templet: function (e) {
                        var distance = "5 米";
                        switch (e.CardReadDistance) {
                            case 0:
                                distance = "1.2 米";
                                break;
                            case 1:
                                distance = "2.5 米";
                                break;
                            case 2:
                                distance = "3.8 米";
                                break;
                        }
                        return distance;
                    }
                },
                {
                    field: 'ReadCardDelay',
                    width: 100,
                    title: '读卡延迟',
                    unresize: true,
                    templet: function (e) {
                        var delay = "5 秒";
                        switch (e.ReadCardDelay) {
                            case 0:
                                delay = "1 秒";
                                break;
                            case 2:
                                delay = "10 秒";
                                break;
                            case 3:
                                delay = "20 秒";
                                break;
                            case 4:
                                delay = "40 秒";
                                break;
                            case 5:
                                delay = "80 秒";
                                break;
                            case 6:
                                delay = "160 秒";
                                break;
                            case 7:
                                delay = "320 秒";
                                break;
                        }
                        return delay;
                    }
                },
                {
                    field: 'CameraDetection',
                    width: 100,
                    title: '车牌识别',
                    unresize: true,
                    templet: function (e) {
                        return e.CameraDetection == 0 ? "关闭" : "开启";
                    }
                },
                {
                    field: 'WirelessNumber',
                    width: 100,
                    title: '无线编号',
                    unresize: true
                },
                {
                    field: 'FrequencyOffset',
                    width: 100,
                    title: '频率偏移',
                    unresize: true
                },
                {
                    field: 'Language',
                    width: 100,
                    title: '语言种类',
                    unresize: true,
                    templet: function (e) {
                        return "普通话";
                    }
                },
                {
                    field: 'FuzzyQuery',
                    width: 120,
                    title: '模糊查询位数',
                    unresize: true,
                    templet: function (e) {
                        return e.FuzzyQuery == 0 ? "关闭" : e.FuzzyQuery;
                    }
                },
                {
                    fixed: 'right',
                    width: 120,
                    title: '操作',
                    align: 'center',
                    toolbar: '#barconfig'
                }
            ]
        ],
        page: false,
    });

    table.on('checkbox(configtable)', function (obj) {
        var checkStatus = table.checkStatus('configtable');
        var btn = $('#btn_config_export');
        if (checkStatus.data.length > 0) {
            if (btn.attr(DISABLED_PROPERTY)) {
                btn.attr(DISABLED_PROPERTY, false);
                btn.removeClass(BUTTON_DISABLED_CLASS);
            }
        } else {
            btn.attr(DISABLED_PROPERTY, true);
            btn.addClass(BUTTON_DISABLED_CLASS);
        }
    })

    table.on("tool(configtable)", function (obj) {
        var data = obj.data;
        if (obj.event == 'edit') {
            // var json = HostPostConfirmInfo(num);
            // var objJson = JSON.parse(json);
            form.val('configform', data);
            ShowConfigDialog("编辑配置信息");
        } else if (obj.event == 'del') {
            layer.confirm("是否删除主机编号：" + data.HostNumber + "的配置信息。", {
                btn: ['删除', '不删除'],
                closeBtn: 2,
                move: false,
                icon: 3,
            }, function (index) {
                var num = obj.tr.index();
                HostPostDelConfirmInfo(num);
                obj.del();
                layer.close(index);
            });
        }
    })

    function ShowConfigDialog(title) {
        windowsIndex = layer.open({
            title: title,
            type: 1,
            move: false,
            closeBtn: 2,
            offset: '100px',
            area: '740px',
            content: $("#config"),
            success: function () {
                EnabledLicensePlateRecognition();
                OpenModelChange();
            },
            end: function () {
                $('#btn_config_reset').trigger('click');
            }
        });
    }

    function EnabledLicensePlateRecognition() {
        var value = $('#licenseplaterecognition').val();
        var disabled = value == 0;
        $("#camera_id").prop(DISABLED_PROPERTY, disabled);
        $("#camera_frequency").prop(DISABLED_PROPERTY, disabled);
    }

    function OpenModelChange() {
        var value = $('#openingmode').val();
        $("#road_gate_id").prop(DISABLED_PROPERTY, (value > 1));
    }

    /* 硬件配置
     *  **************************************************************************************************************
     */

    /* 编号下载
     *  **************************************************************************************************************
     */

    $('#btn_download_number').click(function () {
        var index = $('#download_table tr.layui-bg-select').index();
        var ret = HostPostDownloadNumber(index);
        if (ret) {
            LoadingMsg("编号下载中···");
        }
    })

    $('#btn_download_time').click(function () {
        var ret = HostPostDownloadTime();
        if (ret) {
            LoadingMsg("时间下载中···");
        }
    })

    $('#btn_download_pwd').click(function () {
        windowsIndex = layer.open({
            title: "密码下载",
            type: 1,
            move: false,
            closeBtn: 2,
            area: '410px',
            content: $('#downloadpwd'),
        });
    })

    form.on('checkbox(default_personnel_old_pwd)', function (data) {
        var elem = $("#personnel_old_pwd");
        if (data.elem.checked) {
            elem.val(DEFAULT_DISTANCE_PASSWORD);
        } else {
            elem.val('');
        }
        elem.prop(DISABLED_PROPERTY, data.elem.checked);
    })

    form.on('checkbox(default_personnel_pwd)', function (data) {
        var pwdControl = $('#personnel_pwd');
        var confirmControl = $('#personnel_confirm_pwd');
        var checked = data.elem.checked;
        pwdControl.prop(DISABLED_PROPERTY, checked);
        confirmControl.prop(DISABLED_PROPERTY, checked);
        if (checked) {
            pwdControl.val(DEFAULT_DISTANCE_PASSWORD);
            confirmControl.val(DEFAULT_DISTANCE_PASSWORD);
        } else {
            pwdControl.val('');
            confirmControl.val('');
        }
    })

    form.on('submit(btn_down_pwd_enter)', function (data) {
        var oldpwd = data.field.oldpwd;
        var pwd = data.field.pwd;
        var confirmpwd = data.field.confirmpwd;
        if (pwd != confirmpwd) {
            layer.msg('新密码与确认密码不一致。', {
                anim: 6,
                icon: 2,
            });
            return false;
        }
        var ret = HostPostDownloadPassword(oldpwd, pwd);
        if (ret) {
            LoadingMsg("编号下载中···")
        }
        return false;
    })

    $('#btn_download_show').click(function () {
        var searchContent = $('#search_customer').val();
        var count = HostPostClientInfoCount(searchContent);
        laypage.render({
            elem: 'downloadpager',
            limit: 30,
            count: count,
            layout: ['prev', 'page', 'next', 'count'],
            jump: function (obj, first) {
                var json = HostPostUserInfos(searchContent, obj.curr);
                DisplayClientInfos(json);
            }
        })
    })

    $('#search_customer').keyup(function (evt) {
        if (evt.keyCode == 13) {
            $('#btn_download_show').trigger('click');
        }
    })

    $('#btn_customer_search').click(function () {
        $('#btn_download_show').trigger('click');
    })

    $("#btn_add_ustomer_info").click(function () {
        $('#customer_number').prop(DISABLED_PROPERTY, false);
        ShowcustomerDialog("添加客户信息");
    })

    form.on("submit(btn_info_enter)", function (data) {
        var json = JSON.stringify(data.field);
        var elem = $('#download_table tr.layui-bg-select');
        var index = elem.index();
        if (data.field.Id == 0) {
            var clientNumber = data.field.UserNumber;
            var ret = HostPostConfirmClientNumber(clientNumber);
            if (ret) {
                $('#customer_number').addClass('layui-form-danger').focus();
                layer.msg("当前编号已经存在，请重新输入。", {
                    anim: 6,
                    icon: 2,
                });
                return false;
            }
            json = HostPostAddUserInfo(json);
            // var objJson = JSON.parse(json);
            // var content = ViewDisplayDownloadTableRow(index, objJson);
            // $('#download_table tbody').prepend(content);
            DisplayClientInfos(json);
        } else {
            json = HostPostUpdateUserInfo(json, index);
            var objJson = JSON.parse(json);
            elem.html(ViewDisplayDownloadTableRowContent(index + 1, objJson));
        }
        CloseWindows();
        return false;
    })

    function DisplayClientInfos(json) {
        var objJson = JSON.parse(json);
        var content = "";
        $.each(objJson, function (index, item) {
            content += ViewDisplayDownloadTableRow(index + 1, item);
        });
        $('#download_table tbody').html(content);
    }

    $("#btn_edit_ustomer_info").click(function () {
        $('#customer_number').prop(DISABLED_PROPERTY, true);
        var index = $('#download_table tr.layui-bg-select').index();
        var json = HostPostUserInfo(index);
        var data = JSON.parse(json);
        form.val('infoform', data);
        ShowcustomerDialog("编辑客户信息");
    })

    $("#btn_del_ustomer_info").click(function () {
        layer.confirm("是否删除客户的信息。", {
            btn: ['删除', '不删除'],
            closeBtn: 2,
            move: false,
            icon: 3,
        }, function (index) {
            var elem = $('#download_table tr.layui-bg-select');
            var row = elem.index();
            HostPostDelUserInfo(row);
            elem.remove();
            DownloadControlEnabled(true);
            layer.close(index);
        });
    })

    $("#btn_create_config").click(function () {
        var index = $('#download_table tr.layui-bg-select').index();
        HostPostCreateNumberFile(index);
    })

    $(document).on("click", "a#btn_limit_del", function () {
        var row = $(this).parent().parent('tr').index();
        layer.confirm('确认是否删除当前编号。', {
            btn: ['删除', '不删除'],
            closeBtn: 2,
            icon: 0,
        }, function (index) {
            HostPostDelNumberInfo(row);
            $('#limit_table_tbody').children()[row].remove();
            layer.close(index);
        });
    })

    $(document).on("click", "a#btn_limit_add", function () {
        var index = $(this).parent().parent('tr').index();
        var limitNumber = $('#limit_number').val();
        if (limitNumber.length == 0 || limitNumber.length < 4) {
            $('#limit_number').addClass('layui-form-danger').focus();
            layer.msg('编号不能为空或内容长度不正确。', {
                icon: 2,
                closeBtn: 2,
                anim: 6,
            });
            return;
        }
        var ret = HostPostAddNumberInfo(limitNumber);
        if (!ret) {
            $('#limit_table_tbody').children()[index].remove();
            $('#limit_table_tbody').append("<tr><td>" + limitNumber + "</td><td><a class='layui-btn layui-btn-xs layui-btn-danger' id='btn_limit_del'>删除</a></td></tr><tr><td><input id='limit_number' class='layui-input layui-table-edit' maxlength='4' placeholder='4位数字'></td><td><a class='layui-btn layui-btn-xs' id='btn_limit_add'>添加</a></td></tr>");
        } else {
            $('#limit_number').addClass('layui-form-danger').focus();
            layer.msg('当前编号已经存在，请重新输入。', {
                icon: 2,
                closeBtn: 2,
                anim: 6,
            });
        }
    })

    $("#btn_limit_number").click(function () {
        var json = HostPostLimitNumberInfos();
        var objJson = JSON.parse(json);
        var content = "";
        $.each(objJson, function (index, item) {
            content += "<tr><td>" + item['LimitNumber'] + "</td><td><a class='layui-btn layui-btn-xs layui-btn-danger' id='btn_limit_del'>删除</a></td></tr>";
        });
        content += "<tr><td><input id='limit_number' class='layui-input layui-table-edit' maxlength='4' placeholder='4位数字'></td><td><a class='layui-btn layui-btn-xs' id='btn_limit_add'>添加</a></td></tr>";
        $('#limit_table_tbody').html(content);
        windowsIndex = layer.open({
            title: "限制编号",
            type: 1,
            move: false,
            closeBtn: 2,
            area: '400px',
            content: $("#limit"),
        });
    })

    function ViewDisplayDownloadTableRow(index, json) {
        var content = "<tr>" + ViewDisplayDownloadTableRowContent(index, json) + "</tr>";
        return content;
    }

    function ViewDisplayDownloadTableRowContent(index, json) {
        return "<td>" + index + "</td><td>" + json['Id'] + "</td><td>" + json['UserName'] + "</td><td>" + json['UserNumber'] + "</td><td>" + json['Description'] + "</td><td>" + json['RecordTime'] + "</td>";
    }

    function ShowcustomerDialog(title) {
        windowsIndex = layer.open({
            title: title,
            type: 1,
            move: false,
            closeBtn: 2,
            area: '740px',
            content: $("#info"),
            end: function () {
                $('#btn_donwload_reset').trigger('click');
            }
        });
    }

    /* 编号下载
     *  **************************************************************************************************************
     */

    laydate.render({
        elem: '#personnel_date',
        value: new Date(),
    })

    laydate.render({
        elem: '#issue_date',
        value: new Date(),
    })

    laydate.render({
        elem: '#batch_date',
        value: new Date(),
    })

    laydate.render({
        elem: '#vice_date',
        value: new Date(),
    })

    // laydate.render({
    //     elem: '#customer_time',
    //     value: new Date(),
    // })

})

$(function () {

    /*    导航效果      
     *   *************************************************************************************************************
     */

    var isAnimating = false,
        newScaleValue = 1;

    var dashboard = $(".cd-side-navigation"),
        mainContent = $(".cd-main"),
        loadingBar = $("#cd-loading-bar");

    dashboard.on("click", "li", function (event) {
        event.preventDefault();
        var target = $(this);
        sectionTarget = target.data('menu');
        if (!target.hasClass("selected") && !isAnimating) {
            TriggerAnimation(sectionTarget, true);
        }

        var index = $(this).index();
        if (index == 4) {
            var len = $('#download_table tbody tr').length;
            if (len == 0) {
                $('#btn_download_show').trigger('click');
            }
        }
    });

    function TriggerAnimation(newSection, bool) {
        isAnimating = true;
        newSection = (newSection == '') ? 'index' : newSection;
        dashboard.find('.selected').removeClass('selected');
        dashboard.find('*[data-menu="' + newSection + '"]').addClass('selected');

        InitializeLoadingBar(newSection);

        LoadNewContent(newSection, bool);
    }

    function InitializeLoadingBar(section) {
        var selectedItem = dashboard.find('.selected'),
            barHeight = selectedItem.outerHeight(),
            barTop = selectedItem.offset().top,
            windowHeight = $(window).height(),
            maxOffset = (barTop + barHeight / 2 > windowHeight / 2) ? barTop : windowHeight - barTop - barHeight,
            scaleValue = ((2 * maxOffset + barHeight) / barHeight).toFixed(3) / 1 + 0.001;

        loadingBar.data('scale', scaleValue).css({
            height: barHeight,
            top: barTop
        }).attr('class', '').addClass('loading ' + section);

        setTimeout(function () {
            //LoadingBarAnimation();

            var scaleMax = loadingBar.data('scale');
            loadingBar.velocity({
                scaleY: scaleMax,
            }, 400, function () {
                ResetAfterAnimation();
            });
        }, 50);
    }

    function LoadNewContent(newSection, bool) {
        var section = mainContent.find("." + newSection);
        if (section.length > 0) {
            mainContent.find('.visible').removeClass('visible');
            section.addClass('visible');
        }
    }

    function LoadingBarAnimation() {
        var scaleMax = loadingBar.data('scale');
        if (newScaleValue + 1 < scaleMax) {
            newScaleValue = newScaleValue + 1;
        } else if (newScaleValue + 0.5 < scaleMax) {
            newScaleValue = newScaleValue + 0.5;
        }

        loadingBar.velocity({
            scaleY: newScaleValue,
        }, 100, LoadingBarAnimation);
    }

    function ResetAfterAnimation(newSection) {
        isAnimating = false;

        RsetLoadingBar();
    }

    function RsetLoadingBar() {
        loadingBar.removeClass('loading').velocity({
            scaleY: 1,
        }, 1);
    }

    /*    导航效果      
     *   **************************************************************************************************************
     */

})

$(document).keyup(function (evt) {
    if (evt.keyCode == 123) {
        ShowDevTools();
    }
})

/* 设置 
 *  **************************************************************************************************************
 */

function ViewConnectionFailedMessage() {
    layer.confirm("未能连接到设备，是否重新搜索。", {
        btn: ['重新搜索', '不搜索'],
        icon: 3,
        closeBtn: 2,
        move: false,
    }, function (index) {
        //重新搜索
        HostAutoConnectionDeviced(true);
        layer.close(index);
    });
}

function ViewSerailCountChanged(json) {
    var objJson = JSON.parse(json);
    var selectHtml = "";
    $.each(objJson, function (index, item) {
        selectHtml += "<option value='" + item + "'>" + item + "</option>";
    });
    $("#select_Device1").html(selectHtml);
    $("#select_Device2").html(selectHtml);
    form.render("select", "settingform");

    layer.msg("监测到端口设备发生变化。", {
        offset: "b",
        anim: 1,
        icon: 0,
    });
}

function ViewSerialPortChanged(json) {
    var objJson = JSON.parse(json);
    var open = objJson["IsOpen"];
    var name = objJson["Name"];
    var select = $("select[name=" + name + "]");
    if (open) {
        var portName = objJson["PortName"];
        select.val(portName);
    }
    SettingControlChange(null, select, open);
}

function SettingControlChange(btn, select, state) {
    if (!btn) {
        btn = $("#btn_Device[target=" + select.prop("id") + "]");
    }
    var connModel = $("#connection_mode").val() == "0";
    var deviceName = select.prop("name") == "Device1" ? "发行器" : "人员通道";
    if (state) {
        btn.removeClass("layui-btn-primary").addClass("layui-btn-danger");
        btn.text("关闭");
        if (!connModel) {
            select.prop(DISABLED_PROPERTY, true);
        }

        layer.msg("设备：" + deviceName + " 已连接至端口：" + select.val(), {
            offset: "b",
            anim: 6,
            icon: 1,
        });
    } else {
        btn.removeClass("layui-btn-danger").addClass("layui-btn-primary");
        btn.text("打开");
        if (!connModel) {
            select.prop(DISABLED_PROPERTY, false);
        }

        layer.msg("设备：" + deviceName + " 已打开连接。", {
            offset: "b",
            anim: 6,
            icon: 2,
        });
    }

    if (select.prop("name") == "Device1") {
        DistnaceControlChange(!state);
        EncryptControlEnabled(!state);
        WirelessControlEnabled(!state);
    } else {
        DownloadControlChange(!state);
    }
    form.render("select", "settingform");
}

/* 设置 
 *  **************************************************************************************************************
 */

/* 卡片管理
 *  **************************************************************************************************************
 */

function ViewDisplayCardInfo(json) {
    var count = ($("#card_manager_table_tbody tr").length + 1);
    var content = "<tr>" + DisplayCardContent(json, count) + "</tr>";
    $("#card_manager_table_tbody").append(content);
}

function DisplayCardContent(strJson, count) {
    var objJson = JSON.parse(strJson);
    var id = objJson['Id'];
    var number = objJson['CardNumber'];
    var type = objJson['CardType'];
    var time = formatDate(new Date(objJson['CardTime']));
    var distance = objJson['CardDistance'];
    var lock = objJson['CardLock'];
    var loss = objJson['CardReportLoss'];
    var limit = objJson['ParkingRestrictions'];
    var paration = objJson['CardPartition'];
    var electricity = objJson['Electricity'];

    var imgClass = 'fa-check';
    if (id == 0) {
        imgClass = 'fa-close';
    }

    var typetxt = '单卡';
    switch (type) {
        case -1:
            typetxt = '车牌号码（副卡)';
            break;
        case 1:
            typetxt = '人卡（组合卡）';
            break;
        case 2:
            typetxt = '车牌识别卡';
            break;
        case 3:
            typetxt = '副卡（车卡）';
            break;
        case 4:
            typetxt = '密码错误';
            break;
        case 5:
            typetxt = '挂失卡';
            break;
    }

    var distanceTxt = '主机控制';
    switch (distance) {
        case 1:
            distanceTxt = '1.2 米';
            break;
        case 2:
            distanceTxt = '2.5 米';
            break;
        case 3:
            distanceTxt = '3.8 米';
            break;
        case 4:
            distanceTxt = '5 米';
            break;
    }

    var lockClass = 'fa-unlock';
    if (lock == 1) {
        lockClass = 'fa-lock';
    }

    var lossTxt = "<i class='fa fa-check fa-1x'></i><span> 未挂失</span>";
    if (loss == 1) {
        lossTxt = "<i class='fa fa-close fa-1x'></i><span> 已挂失</span>";
    }

    var limitClass = 'fa-close';
    if (limit == 1) {
        limitClass = "fa-check";
    }

    var partitionTxt = '未设置分区';
    if (paration > 0) {
        partitionTxt = PartitionToStr(paration);
    }

    var electricityClass = 'fa-battery';
    if (electricity == 1) {
        electricityClass = 'fa-battery-quarter';
    }

    var content = "<td>" + count + "</td><td align='center'><i class='fa " + imgClass + " fa-1x'></i></td ><td>" + number + "</td><td>" + typetxt + "</td><td>" + time + "</td><td>" + distanceTxt + "</td><td align='center'><i class='fa " + lockClass + " fa-1x'></i></td><td align='center'>" + lossTxt + "</td><td align='center'><i class='fa " + limitClass + " fa-1x'></i></td><td>" + partitionTxt + "</td><td align='center'><i class='fa " + electricityClass + " fa-1x'></i></td>";
    return content;
}

function ViewViceRemoveLock(json) {
    var objJson = JSON.parse(json);
    form.reload('vicetable', {
        data: objJson,
    });
}

function ViewIssueOver(json) {
    CloseLoading();

    var content = DisplayCardContent(json, rowIndex + 1);
    var elem = $('#card_manager_table_tbody').children()[rowIndex];
    $(elem).html(content);

    CloseWindows();
}

function ViewBatchOver(count) {
    CloseLoading();
    if (count == 0) {
        CloseWindows();
    }
}

function ViewDisplayBatchContent(json, index) {
    var row = parseInt(index);
    var content = DisplayCardContent(json, row + 1);
    var elem = $('#card_manager_table_tbody').children()[row];
    $(elem).html(content);
}

function ViewPersonnelIssueOver(json) {
    CloseLoading();

    if (json.length > 0) {
        var index = parseInt($('#personnel_index').val());
        var content = DisplayCardContent(json, index + 1);
        var elem = $('#card_manager_table_tbody').children()[index];
        $(elem).html(content);

        CloseWindows();
    }
}

function DistnaceControlChange(enabled) {
    var btnRefresh = $('#btn_refresh').prop(DISABLED_PROPERTY, enabled);
    var btnPersonnelIssue = $('#btn_personnel_issue').prop(DISABLED_PROPERTY, enabled);
    var btnPersonnelBatch = $('#btn_personnel_batch').prop(DISABLED_PROPERTY, enabled);
    var btnlose = $('#btn_lose').prop(DISABLED_PROPERTY, enabled);
    var btnrecovery = $('#btn_recovery').prop(DISABLED_PROPERTY, enabled);
    var btnIssue = $('#btn_issue');
    var btnBatch = $('#btn_batch');

    if (enabled) {
        btnRefresh.addClass(BUTTON_DISABLED_CLASS);
        btnIssue.addClass(BUTTON_DISABLED_CLASS);
        btnBatch.addClass(BUTTON_DISABLED_CLASS);
        btnPersonnelIssue.addClass(BUTTON_DISABLED_CLASS);
        btnPersonnelBatch.addClass(BUTTON_DISABLED_CLASS);
        btnlose.addClass(BUTTON_DISABLED_CLASS);
        btnrecovery.addClass(BUTTON_DISABLED_CLASS);
    } else {
        btnRefresh.removeClass(BUTTON_DISABLED_CLASS);
        btnBatch.removeClass(BUTTON_DISABLED_CLASS);
        btnPersonnelBatch.removeClass(BUTTON_DISABLED_CLASS);
        btnlose.removeClass(BUTTON_DISABLED_CLASS);
        btnrecovery.removeClass(BUTTON_DISABLED_CLASS);
        if (rowIndex > -1) {
            btnIssue.removeClass(BUTTON_DISABLED_CLASS);
            btnPersonnelIssue.removeClass(BUTTON_DISABLED_CLASS);
            return;
        }
    }
    btnIssue.prop(DISABLED_PROPERTY, enabled);
    btnBatch.prop(DISABLED_PROPERTY, enabled);
}

function ViewLoseOver() {
    CloseLoading();

    CloseWindows();
}

/* 卡片管理
 *  **************************************************************************************************************
 */

/* 加密管理
 *  **************************************************************************************************************
 */

function EncryptControlEnabled(enabled) {
    $('#distance_default_old_pwd').prop(DISABLED_PROPERTY, enabled);
    $('#distance_default_pwd').prop(DISABLED_PROPERTY, enabled);
    $('#distance_default_number').prop(DISABLED_PROPERTY, enabled);
    $('#distance_card').prop(DISABLED_PROPERTY, enabled);
    $('#distance_device').prop(DISABLED_PROPERTY, enabled);

    $('#ic_default_old_pwd').prop(DISABLED_PROPERTY, enabled);
    $('#ic_default_pwd').prop(DISABLED_PROPERTY, enabled);
    $('#ic_card').prop(DISABLED_PROPERTY, enabled);
    $('#ic_device').prop(DISABLED_PROPERTY, enabled);

    var btnDistanceEncrypt = $('#btn_distance_encrypt').prop(DISABLED_PROPERTY, enabled);
    var btnIcEncrypt = $('#btn_ic_encrypt');
    if (enabled) {
        $('#distance_old_pwd').prop(DISABLED_PROPERTY, true);
        $('#distance_pwd').prop(DISABLED_PROPERTY, true);
        $('#distance_confirm_pwd').prop(DISABLED_PROPERTY, true);
        $('#distance_client_number').prop(DISABLED_PROPERTY, true);

        $('#ic_old_pwd').prop(DISABLED_PROPERTY, true);
        $('#ic_pwd').prop(DISABLED_PROPERTY, true);
        $('#ic_confirm_pwd').prop(DISABLED_PROPERTY, true);

        btnDistanceEncrypt.addClass(BUTTON_DISABLED_CLASS);
        if (!btnIcEncrypt.hasClass(BUTTON_DANGER_CLASS)) {
            btnIcEncrypt.prop(DISABLED_PROPERTY, enabled).addClass(BUTTON_DISABLED_CLASS);
        }

    } else {
        if (!$('#distanceoldpwdgroup').is(':hidden')) {
            if (!$('#distance_default_old_pwd').prop('checked')) {
                $('#distance_old_pwd').prop(DISABLED_PROPERTY, false);
            }
        }
        if (!$('#distance_default_pwd').prop('checked')) {
            $('#distance_pwd').prop(DISABLED_PROPERTY, false);
            $('#distance_confirm_pwd').prop(DISABLED_PROPERTY, false);
        }
        if (!$('#distance_default_number').prop('checked')) {
            $('#distance_client_number').prop(DISABLED_PROPERTY, false);
        }
        if (!$('#icoldpwdgroup').is(':hidden')) {
            if (!$('#ic_default_old_pwd').prop('checked')) {
                $('#ic_old_pwd').prop(DISABLED_PROPERTY, false);
            }
        }
        if (!$('#ic_default_pwd').prop('checked')) {
            $('#ic_pwd').prop(DISABLED_PROPERTY, false);
            $('#ic_confirm_pwd').prop(DISABLED_PROPERTY, false);
        }

        btnDistanceEncrypt.removeClass(BUTTON_DISABLED_CLASS);
        if (btnIcEncrypt.hasClass(BUTTON_DANGER_CLASS)) {
            btnIcEncrypt.text('确认');
            btnIcEncrypt.removeClass(BUTTON_DANGER_CLASS);
        } else {
            btnIcEncrypt.prop(DISABLED_PROPERTY, enabled).removeClass(BUTTON_DISABLED_CLASS);
        }

    }
    form.render(null, 'distanceform');
    form.render(null, 'icform');
}

function ViewEncryptOver() {
    EncryptControlEnabled(false);
}

function ViewEncryptMessage(msg) {
    var content = "<div class='news-container'><span>" + msg + "</span><i>" + formatDateTime(new Date()) + "</i></div>";
    $('#password_news').prepend(content);
}

/* 加密管理
 *  **************************************************************************************************************
 */

/* 无线测试
 *  **************************************************************************************************************
 */

function ViewWirelessMessage(msg) {
    $('#wireless_news').prepend("<div class='news-container'><span>" + msg + "</span><i>" + formatDateTime(new Date()) + "</i></div>");
}

function ViewWirelessOver() {
    WirelessControlEnabled(false);
}

function WirelessControlEnabled(enabled) {
    $('#wireless_id').prop(DISABLED_PROPERTY, enabled);
    $('#radio_frequency').prop(DISABLED_PROPERTY, enabled);
    $('#btn_Wireless_setup').prop(DISABLED_PROPERTY, enabled);
    $('#btn_wireless_test').prop(DISABLED_PROPERTY, enabled);
    $('#btn_wireless_query').prop(DISABLED_PROPERTY, enabled);

    if (enabled) {
        $('#btn_Wireless_setup').addClass(BUTTON_DISABLED_CLASS);
        $('#btn_wireless_test').addClass(BUTTON_DISABLED_CLASS);
        $('#btn_wireless_query').addClass(BUTTON_DISABLED_CLASS);
    } else {
        $('#btn_Wireless_setup').removeClass(BUTTON_DISABLED_CLASS);
        $('#btn_wireless_test').removeClass(BUTTON_DISABLED_CLASS);
        $('#btn_wireless_query').removeClass(BUTTON_DISABLED_CLASS);
    }

    var btnWirelessSearch = $('#btn_wireless_search');
    if (btnWirelessSearch.hasClass(BUTTON_DANGER_CLASS)) {
        if (!enabled) {
            btnWirelessSearch.removeClass(BUTTON_DANGER_CLASS);
            btnWirelessSearch.text('搜索');
        }
    } else {
        btnWirelessSearch.prop(DISABLED_PROPERTY, enabled);
        if (enabled) {
            btnWirelessSearch.addClass(BUTTON_DISABLED_CLASS);
        } else {
            btnWirelessSearch.removeClass(BUTTON_DISABLED_CLASS);
        }
    }
    var btnRefreshIc = $('#btn_refresh_ic');
    if (btnRefreshIc.hasClass(BUTTON_DANGER_CLASS)) {
        if (!enabled) {
            btnRefreshIc.removeClass(BUTTON_DANGER_CLASS);
            btnRefreshIc.text('刷IC卡');
        }
    } else {
        btnRefreshIc.prop(DISABLED_PROPERTY, enabled);
        if (enabled) {
            btnRefreshIc.addClass(BUTTON_DISABLED_CLASS);
        } else {
            btnRefreshIc.removeClass(BUTTON_DISABLED_CLASS);
        }
    }
}

/* 无线测试
 *  **************************************************************************************************************
 */


/* 编号下载
 *  **************************************************************************************************************
 */

function ViewDownloadOver(ret) {
    CloseLoading();
    if (ret == 'True') {
        layer.msg('下载成功。', {
            icon: 1,
            anim: 6,
        });
        if (windowsIndex > -1) {
            CloseWindows();
        }
    } else {
        layer.msg('下载失败，请重新操作。', {
            icon: 2,
            anim: 6,
        });
    }
}

function DownloadControlEnabled(enabled) {
    var btnEdit = $('#btn_edit_ustomer_info').prop(DISABLED_PROPERTY, enabled);
    var btnDel = $('#btn_del_ustomer_info').prop(DISABLED_PROPERTY, enabled);
    var btnCreate = $('#btn_create_config').prop(DISABLED_PROPERTY, enabled);

    if (enabled) {
        btnEdit.addClass(BUTTON_DISABLED_CLASS);
        btnDel.addClass(BUTTON_DISABLED_CLASS);
        btnCreate.addClass(BUTTON_DISABLED_CLASS);
    } else {
        if (btnEdit.hasClass(BUTTON_DISABLED_CLASS)) {
            btnEdit.removeClass(BUTTON_DISABLED_CLASS);
            btnDel.removeClass(BUTTON_DISABLED_CLASS);
            btnCreate.removeClass(BUTTON_DISABLED_CLASS);
        }
    }
    DownloadNumberControlEnabled(enabled);
}

function DownloadControlChange(enabled) {
    var btnTime = $('#btn_download_time').prop(DISABLED_PROPERTY, enabled);
    var btnPassword = $('#btn_download_pwd').prop(DISABLED_PROPERTY, enabled);
    if (enabled) {
        btnTime.addClass(BUTTON_DISABLED_CLASS);
        btnPassword.addClass(BUTTON_DISABLED_CLASS);
    } else {
        btnTime.removeClass(BUTTON_DISABLED_CLASS);
        btnPassword.removeClass(BUTTON_DISABLED_CLASS);
    }
    DownloadNumberControlEnabled(enabled);
}

function DownloadNumberControlEnabled(enabled) {
    var btnNumber = $('#btn_download_number');
    if (enabled) {
        btnNumber.prop(DISABLED_PROPERTY, enabled).addClass(BUTTON_DISABLED_CLASS);
    } else {
        if (!$('#btn_download_time').prop(DISABLED_PROPERTY)) {
            var index = $('#download_table tr.layui-bg-select').index();
            if (index > -1) {
                btnNumber.prop(DISABLED_PROPERTY, enabled).removeClass(BUTTON_DISABLED_CLASS);
            }
        }
    }
}

/* 编号下载
 *  **************************************************************************************************************
 */

/* 公共方法
 *  **************************************************************************************************************
 */

function LoadingMsg(msg) {
    loadingIndex = layer.msg(msg, {
        icon: 16,
        time: 0,
        shade: 0.5,
    });
}

function CloseLoading() {
    layer.close(loadingIndex);
}

function CloseWindows() {
    layer.close(windowsIndex);
    windowsIndex = -1;
}

function formatTen(num) {
    return num > 9 ? (num + "") : ("0" + num);
}

function formatDate(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    // var hour = date.getHours();
    // var minute = date.getMinutes();
    // var second = date.getSeconds();
    return year + "-" + formatTen(month) + "-" + formatTen(day);
}

function formatDateTime(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hour = date.getHours();
    var minute = date.getMinutes();
    var second = date.getSeconds();
    return year + "-" + formatTen(month) + "-" + formatTen(day) + " " + formatTen(hour) + ":" + formatTen(minute) + ":" + formatTen(second);
}

function PartitionToInt(array) {
    var num = 0;
    $.each(array, function (index, item) {
        num += (1 << (item - 1));
    });
    return num;
}

function PartitionToArray(partition) {
    var parts = new Array();
    var number = 1;
    var index = 0;
    while (partition >= 1) {
        var num = Math.floor(partition % 2);
        if (num > 0) {
            parts[index] = number;
            index += 1;
        }
        partition = Math.floor(partition / 2);
        number += 1;
    }
    return parts;
}

function PartitionToStr(partition) {
    var txt = '';
    var array = PartitionToArray(partition);
    $.each(array, function (index, item) {
        txt += "分区 " + item + " ";
    });
    return txt;
}

function ViewMessage(msg, icon) {
    layer.msg(msg, {
        icon: icon,
    });
}

function ViewAlert(msg) {
    layer.alert(msg, {
        title: "提示",
        icon: 2,
        move: false,
        closeBtn: 2,
    });
}