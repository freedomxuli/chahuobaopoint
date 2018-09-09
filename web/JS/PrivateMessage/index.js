var pageSize = 20;
var currentPage = 1;
var GridStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    autoLoad: true,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'ISREADED', type: 'bool' },
       { name: 'TITLE', type: 'string' },
       { name: 'CREATETIME', type: 'date' },
       { name: 'ID', type: 'string' },
       { name: 'USERSNAME', type: 'string' },
       { name: 'USERSDEPTNAME', type: 'string' },
       { name: 'USERSORGNAME', type: 'string' },
       { name: 'MSGTYPE', type: 'number' },
       { name: 'MAILBOX', type: 'number' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        BindStore(nPage);
    }
});
//inline_include("approot/r/js/privatemessage/pm.js");
var msgLib = window.top.msgLib;
function BindStore(nPage) {
    var readStatus = Ext.getCmp('cmbReadStatus').getValue();
    var msgType = Ext.getCmp('cmbMsgType').getValue();
    var mailBox = Ext.getCmp('cmbMailBox').getValue();
    CS('CZCLZ.PM.GetUserMessage', function (retVal) {
        GridStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
        currentPage = retVal.cp;
    }, CS.onError, nPage, pageSize, readStatus, msgType, mailBox);
}

Ext.define('messageView', {
    extend: 'Ext.window.Window',
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    width: 600,
    modal: true,
    resizable: false,
    title: '查看',
    buttons: [
        {
            text: '标记已读',
            id:'btnMarkReaded',
            handler: function () {
                var dispWin = this.up('window');
                Ext.Msg.confirm('提醒', '是否标记为已读状态？', function (btn) {
                    if (btn == 'yes') {
                        var messageId = Ext.getCmp('hdMessageId').getValue();
                        msgLib.markReaded(messageId);
                        var record = GridStore.findRecord('ID', messageId);
                        if (record) {
                            record.set('ISREADED', true);
                            record.commit();
                        }
                        dispWin.close();

                    }
                });
            }
        },
        {
            text: '回复',
            id: 'btnReplyMsg',
            handler: function () {
                var dispWin = this.up('window');
                var fromId = Ext.getCmp('hdFromId').getValue();
                var messageId = Ext.getCmp('hdMessageId').getValue();
                dispWin.close();
                msgLib.replyMsg(fromId, messageId);
            }
        },
        {
            text: '办理',
            id: 'btnGoto'
        }
    ],
    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    border:false,
                    bodyPadding: 3,
                    items: [
                        {
                            xtype: 'hiddenfield',
                            id: 'hdFromId'
                        },
                        {
                            xtype: 'hiddenfield',
                            id: 'hdMessageId'
                        },
                        {
                            xtype: 'displayfield',
                            fieldLabel: '发件人',
                            labelWidth: 80,
                            id: 'dispFrom'
                        },
                        {
                            xtype: 'displayfield',
                            labelWidth: 80,
                            fieldLabel: '时　间',
                            id: 'dispCreateTime'
                        },
                        {
                            xtype: 'displayfield',
                            labelWidth: 80,
                            fieldLabel: '收件人',
                            id: 'dispTo'
                        },
                        {
                            xtype: 'displayfield',
                            labelWidth: 80,
                            fieldLabel: '标题',
                            id: 'dispTitle'
                        },
                        {
                            xtype: 'component',
                            id: 'dispContent',
                            height: 80,
                            padding: 6,
                            autoScroll: true
                        }
                    ]
                }
            ]
        });
        me.callParent(arguments);
    }
});


function ShowMessage(v,isReaded,mailBox) {
    CS('CZCLZ.PM.GetUserMessageDetailWithoutChangeStatus', function (retVal) {
        if (retVal.MessageDetail && retVal.MessageDetail.length > 0) {
            var msgWin = Ext.create('messageView');
            Ext.getCmp('dispFrom').setValue('<b>' + retVal.MessageDetail[0].FSR_MC + '</b>'
                    + (retVal.MessageDetail[0].FSR_DW_MC ? '&nbsp;&nbsp;&nbsp;&nbsp;' + retVal.MessageDetail[0].FSR_DW_MC : "")
                    + (retVal.MessageDetail[0].FSR_BM_MC ? "（<i>" + retVal.MessageDetail[0].FSR_BM_MC + "</i>）" : ""));
            Ext.getCmp('dispCreateTime').setValue(retVal.MessageDetail[0].ADDTIME.toCHString(true));
            Ext.getCmp('dispTo').setValue(retVal.MessageDetail[0].JSR_XX.replace(/;/g, '，'));
            Ext.getCmp('dispTitle').setValue(retVal.MessageDetail[0].ZNXX_BT ? retVal.MessageDetail[0].ZNXX_BT : "无");
            Ext.getCmp('dispContent').update(retVal.MessageDetail[0].ZNXX_NR);
            Ext.getCmp('hdFromId').setValue(retVal.MessageDetail[0].FSR_ID ? retVal.MessageDetail[0].FSR_ID : "");
            Ext.getCmp('hdMessageId').setValue(retVal.MessageDetail[0].ZNXX_ID);
            var msgType = retVal.MessageDetail[0].ZNXX_LX;
            if (msgType == 0)
                Ext.getCmp('btnReplyMsg').setDisabled(true);

            if (mailBox == 0)
                Ext.getCmp('btnReplyMsg').setVisible(true);
            else
                Ext.getCmp('btnReplyMsg').setVisible(false);

            if (mailBox == 0 && !isReaded) {
                Ext.getCmp('btnMarkReaded').setVisible(true);
            }
            else {
                Ext.getCmp('btnMarkReaded').setVisible(false);
            }

            var tlb = msgWin.query('toolbar');
            if (retVal.MessageDetail[0].ZNXX_URL && retVal.MessageDetail[0].ZNXX_LX == 0) {
                var btnGoto = Ext.getCmp('btnGoto');
                btnGoto.setVisible(true);
                btnGoto.setHandler(function () {
                    msgWin.close();
                    window.openAppUrl(retVal.MessageDetail[0].ZNXX_APPNAME, retVal.MessageDetail[0].ZNXX_URL);
                });
            }
            else {
                var btnGoto = Ext.getCmp('btnGoto');
                btnGoto.setVisible(false);
            }
            msgWin.show();
        }
    }, CS.onError, v);
}
Ext.onReady(function () {
    Ext.create('Ext.container.Viewport', {
        layout: {
            type: 'border'
        },
        items: [
            {
                region: 'center',
                xtype: 'panel',
                title: '消息管理',
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                items: [
                    {
                        xtype: 'gridpanel',
                        store: GridStore,
                        id: 'gridMain',
                        flex: 1,
                        selType: 'checkboxmodel',
                        dockedItems: [
                            {
                                xtype: 'pagingtoolbar',
                                store: GridStore,
                                dock: 'bottom',
                                displayInfo: true
                            }
                        ],
                        viewConfig: {
                            getRowClass: function (rec, rowIdx, params, store) {
                                if (rec.get('MAILBOX') == 0) {
                                    return rec.get('ISREADED') ? '' : 'bold-row';
                                }
                            }
                        },
                        tbar: [
                            {
                                xtype: 'combobox',
                                fieldLabel: '查看',
                                valueField: 'val',
                                displayField: 'disp',
                                queryMode: 'local',
                                id: 'cmbMailBox',
                                editable: false,
                                labelWidth: 50,
                                width: 200,
                                value: 0,
                                store: new Ext.data.ArrayStore({
                                    fields: ['disp', 'val'],
                                    data: [
                                        ['收件箱', 0],
                                        ['发件箱', 1]
                                    ]
                                }),
                                listeners: {
                                    change: function (t, n, o) {
                                        if (n == 1) {
                                            Ext.getCmp('cmbReadStatus').hide();
                                            this.up('grid').columns[4].setText('收件人');
                                            BindStore(1);
                                        }
                                        else {
                                            Ext.getCmp('cmbReadStatus').show();
                                            this.up('grid').columns[4].setText('发件人');
                                            BindStore(1);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'combobox',
                                fieldLabel: '阅读状态',
                                valueField: 'val',
                                displayField: 'disp',
                                queryMode: 'local',
                                id: 'cmbReadStatus',
                                labelWidth: 70,
                                editable: false,
                                store: new Ext.data.ArrayStore({
                                    fields: ['disp', 'val'],
                                    data: [
                                        ['已读', 1],
                                        ['未读', 0]
                                    ]
                                })
                            },
                            {
                                xtype: 'combobox',
                                fieldLabel: '消息类型',
                                valueField: 'val',
                                displayField: 'disp',
                                queryMode: 'local',
                                id: 'cmbMsgType',
                                labelWidth: 70,
                                editable: false,
                                store: new Ext.data.ArrayStore({
                                    fields: ['disp', 'val'],
                                    data: [
                                        ['通知', 0],
                                        ['消息', 1]
                                    ]
                                })
                            },
                            {
                                xtype: 'button',
                                text: '查询',
                                iconCls: 'search',
                                handler: function () {
                                    BindStore(0);
                                }
                            },
                            {
                                xtype: 'button',
                                text: '新增',
                                iconCls: 'add',
                                handler: function () {
                                    msgLib.sendMsg();
                                }
                            },
                            {
                                xtype: 'button',
                                text: '删除',
                                iconCls: 'delete',
                                handler: function () {
                                    var idlist = [];
                                    var grid = Ext.getCmp("gridMain");
                                    var rds = grid.getSelectionModel().getSelection();
                                    if (rds.length == 0) {
                                        Ext.Msg.show({
                                            title: '提示',
                                            msg: '请选择至少一条要删除的记录!',
                                            buttons: Ext.MessageBox.OK,
                                            icon: Ext.MessageBox.INFO
                                        });
                                        return;
                                    }
                                    Ext.MessageBox.confirm("提示", "是否删除你所选记录？", function (obj) {
                                        var mailBox = Ext.getCmp('cmbMailBox').getValue();
                                        if (obj == "yes") {
                                            for (var n = 0, len = rds.length; n < len; n++) {
                                                var rd = rds[n];
                                                idlist.push(rd.get("ID"));
                                            }
                                            CS('CZCLZ.PM.DeleteMsg', function (retVal) {
                                                Ext.Msg.alert('提醒', '删除成功！');
                                                BindStore(1);
                                            }, CS.onError, idlist, mailBox);
                                        }
                                    });
                                }
                            }
                        ],
                        columns: [
                            {
                                text: '状态',
                                sortable: false,
                                menuDisabled: true,
                                align: 'center',
                                width: 80,
                                dataIndex: 'ISREADED',
                                renderer: function (v, m) {
                                    if (!v)
                                        return "未读";
                                    else
                                        return "已读";
                                }
                            },
                            {
                                text: '类型',
                                sortable: false,
                                menuDisabled: true,
                                width: 80,
                                align: 'center',
                                dataIndex: 'MSGTYPE',
                                renderer: function (v, m) {
                                    if (v == 0)
                                        return "通知";
                                    else if (v == 1)
                                        return "消息";
                                }
                            },
                            {
                                text: '标题',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                dataIndex: 'TITLE'
                            },
                            {
                                text: '内容',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                dataIndex: 'CONTENT'
                            },
                            {
                                text: '发件人',
                                sortable: false,
                                menuDisabled: true,
                                width: 320,
                                dataIndex: 'USERSNAME',
                                renderer: function (v, m) {
                                    if (m.record.data.MAILBOX == '0') {
                                        var editStr = '<b>' + v + "</b>&nbsp;&nbsp;" + m.record.data.USERSORGNAME + "（<i>" + m.record.data.USERSDEPTNAME + "</i>）";
                                        return editStr;
                                    }
                                    else {
                                        var editStr = '<b>' + v.replace(/;/g, '，') + '</b>';
                                        return editStr;
                                    }
                                }
                            },
                            {
                                text: '时间',
                                sortable: false,
                                menuDisabled: true,
                                xtype: 'datecolumn',
                                format: 'Y年m月d日 H:i:s',
                                width: 180,
                                dataIndex: 'CREATETIME'
                            },
                            {
                                text: '操作',
                                sortable: false,
                                menuDisabled: true,
                                width: 180,
                                align: 'center',
                                dataIndex: 'ID',
                                renderer: function (v, m) {
                                    var editStr = "";
                                    editStr += "<a href='javascript:void(0);' onclick='ShowMessage(\"" + v + "\"," + m.record.data.ISREADED + "," + m.record.data.MAILBOX + ");'>查看</a>";
                                    return editStr;
                                }
                            }
                        ]
                    }
                ]
            }
        ]
    });
    BindStore(1);
});