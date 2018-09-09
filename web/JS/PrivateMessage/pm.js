

Ext.define('SendShortMsgDialog', {
    extend: 'Ext.window.Window',
    layout: {
        type: 'fit'
    },
    width: 620,
    height:500,
    resizable: false,
    modal: true,
    title: '站内短信',
    initComponent: function () {
        var me = this;
        var recentContactStore = Ext.create('Ext.data.Store', {
            fields: [
                {
                    name: 'text',
                    type:'string'
                },
                {
                    name: 'id',
                    type:'string'
                }
            ]
        });

        var userStore = Ext.create('Ext.data.TreeStore', {
            fields: ['id', 'text', 'flag', 'expanded', 'leaf'],
            root: {
                expanded: true,
                id: 'root'
            }
        });

        var tagStore = Ext.create('Ext.data.Store', {
            fields: [
                {
                    name: 'text',
                    type: 'string'
                },
                {
                    name: 'id',
                    type: 'string'
                }
            ]
        });

        var treeStr;
        InlineCS('CZCLZ.PM.GetUserList', function (retVal) {
            recentContactStore.loadData(retVal.RecenctContactUser);
            userStore.getRootNode().appendChild(retVal.ContactUser.evalJSON());
        }, CS.onError);

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'form',
                    layout: 'border',
                    border:false,
                    items: [
                        {
                            xtype: 'panel',
                            region: 'east',
                            split: true,
                            hideCollapseTool: true,
                            header :false,
                            width:180,
                            collapsible: true,
                            border: false,
                            collapseMode: 'mini',
                            layout: {
                                type: 'vbox',
                                align: 'stretch'
                            },
                            items:[
                                {
                                    xtype: 'grid',
                                    title: '最近联系人',
                                    store: recentContactStore,
                                    hideHeaders: true,
                                    rowLines: false,
                                    columns: [
                                        {
                                            text: '姓名',
                                            dataIndex: 'text',
                                            flex: 1
                                        }
                                    ],
                                    height: 160,
                                    listeners: {
                                        itemclick: function (t, r) {
                                            var tagField = Ext.getCmp('receiverTag');
                                            var tagStore = tagField.getStore();
                                            var record = tagStore.findRecord('id', r.data.id);
                                            if (record) {
                                                tagField.addValue(record.data.id);
                                            }
                                            else {
                                                tagStore.add({ text: r.data.text, id: r.data.id });
                                                tagField.addValue(r.data.id);
                                            }
                                        }
                                    }
                                },
                                {
                                    xtype: 'treepanel',
                                    scrollable: 'y',
                                    flex: 1,
                                    rootVisible: false,
                                    id:'treeContact',
                                    title: '单位联系人',
                                    store: userStore,
                                    listeners: {
                                        itemclick: function (t,r) {
                                            if (r.data.flag) {
                                                var tagField = Ext.getCmp('receiverTag');
                                                var tagStore = tagField.getStore();
                                                var record = tagStore.findRecord('id', r.data.id);
                                                if (record) {
                                                    tagField.addValue(record.data.id);
                                                }
                                                else {
                                                    tagStore.add({ text: r.data.text, id: r.data.id });
                                                    tagField.addValue(r.data.id);
                                                }
                                            }
                                        }
                                    }
                                }
                            ]

                        },
                        {
                            xtype: 'panel',
                            region: 'center',
                            layout: {
                                type: 'vbox'
                            },
                            bodyPadding:10,
                            items:[
                                {
                                    xtype: 'boxselect',
                                    fieldLabel: '收件人',
                                    width: '100%',
                                    displayField: 'text',
                                    valueField: 'id',
                                    labelWidth:60,
                                    id: 'receiverTag',
                                    queryMode: 'local',
                                    store: tagStore
                                },
                                {
                                    xtype: 'textfield',
                                    labelWidth: 60,
                                    id:'txtTitle',
                                    fieldLabel: '主题',
                                    width: '100%'
                                },
                                {
                                    xtype: 'htmleditor',
                                    fieldLabel: '正文',
                                    id: 'txtContent',
                                    labelWidth: 60,
                                    width: '100%',
                                    flex:1
                                }
                            ],
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '发送',
                                    handler: function () {
                                        var idArray = Ext.getCmp('receiverTag').getValue();
                                        if (idArray.length == 0) {
                                            Ext.Msg.alert('提醒', '未选择收件人，选择后继续！');
                                            return;
                                        }
                                        var nameArray = Ext.getCmp('receiverTag').getRawValue().split(',');
                                        var sendTo = [];
                                        for (var i = 0; i < idArray.length; i++) {
                                            sendTo.push({ name: nameArray[i], id: idArray[i] });
                                        }
                                        var title = Ext.getCmp('txtTitle').getValue();
                                        var content = Ext.getCmp('txtContent').getValue();
                                        if (Ext.String.trim(content) == '' && Ext.String.trim(title) == '') {
                                            Ext.Msg.alert('提醒', '标题与内容不能同时为空，请修改后继续');
                                            return;
                                        }
                                        CS('CZCLZ.PM.SendShortMsg', function () {
                                            Ext.Msg.alert('提醒', '发送成功！');
                                            me.close();
                                        }, CS.onError, title, content, sendTo);
                                    }
                                },
                                {
                                    text: '取消',
                                    handler: function () {
                                        me.close();
                                    }
                                }
                            ]
                        }
                    ]
                }
            ]
        });
        me.callParent(arguments);
        if (me.userId) {
            var tagField = Ext.getCmp('receiverTag');
            var record = tagStore.findRecord('id', me.userId);
            if (record) {
                tagField.addValue(record.data.id);
            }
            else {
                var r = Ext.getCmp('treeContact').getRootNode();
                r.cascadeBy(function (childNode) {
                    if (childNode.get('id') == me.userId) {
                        tagStore.add({ text: childNode.get('text'), id: childNode.get('id') });
                        tagField.addValue(childNode.get('id'));
                    }
                });
            }
        }
    }
});

var msgLib = {
    viewMsg: function (msgId) {
        var viewWindow = Ext.getCmp('msgViewDialog');
        if (!viewWindow) {
            viewWindow = Ext.create('Ext.window.Window', {
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                width: 600,
                resizable: false,
                id: 'msgViewDialog',
                closeAction: 'hide',
                title: '查看',
                buttons: [
                    /*{
                        text: '标记已读',
                        handler: function () {
                            var dispWin = this.up('window');
                            Ext.Msg.confirm('提醒', '是否标记为已读状态？', function (btn) {
                                if (btn == 'yes') {
                                    var messageId = Ext.getCmp('hdMessageId').getValue();
                                    msgLib.markReaded(messageId);
                                    dispWin.close();
                                }
                            });
                        }
                    },*/
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
                        id: 'btnGoto',
                        hidden:true
                    }
                ],
                items: [
                    {
                        xtype: 'panel',
                        bodyPadding: 3,
                        border:false,
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
        }
        if (viewWindow.isHidden())
            viewWindow.show();
        ACS('CZCLZ.PM.GetUserMessageDetail', function (retVal) {
            if (retVal.MessageDetail && retVal.MessageDetail.length > 0) {
      
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
               
                //if (msgType == 0)
                //    Ext.getCmp('btnReplyMsg').setDisabled(true);
                //if (retVal.Attachment && retVal.Attachment.length > 0) {
                //    Ext.getCmp('dvAttachment').show();
                //    Ext.getCmp('dvAttachment').getStore().loadData(retVal.Attachment);
                //}
                //else {
                //    Ext.getCmp('dvAttachment').hide();
                //}
                var tlb = viewWindow.query('toolbar');

                if (retVal.MessageDetail[0].ZNXX_URL && retVal.MessageDetail[0].ZNXX_LX == 0) {
                    var btnGoto = Ext.getCmp('btnGoto');
                    btnGoto.setVisible(true);
                    btnGoto.setHandler(function () {
                        viewWindow.close();
                        window.openAppUrl(retVal.MessageDetail[0].ZNXX_APPNAME, retVal.MessageDetail[0].ZNXX_URL);
                    });
                }
                else {
                    var btnGoto = Ext.getCmp('btnGoto');
                    btnGoto.setVisible(false);
                }
            }
        }, CS.onError, msgId);
    },
    sendMsg: function () {
        var sendMsgDialog = Ext.create('SendShortMsgDialog');
        sendMsgDialog.show();
    },
    replyMsg: function (v, msgId) {
        ACS('CZCLZ.PM.MarkMessagesReaded', function () {
            var sendMsgDialog = Ext.create('SendShortMsgDialog', { userId: v });
            sendMsgDialog.show();
        }, CS.onError, [msgId]);
    },
    markAllReaded: function () {
        var msgList = $('.notify-dialog .content li');
        if (msgList.length > 0) {
            var msgIds = [];
            for (var i = 0; i < msgList.length; i++) {
                var msgId = $(msgList[i]).data('msgid');
                msgIds.push(msgId);
            }
            ACS('CZCLZ.PM.MarkMessagesReaded', function () {
                $('.notify-dialog').hide();
            }, CS.onError, msgIds);
        }
    },
    isClose: false,
    close: function () {
        this.isClose = true;
        $('.notify-dialog').hide();
    },
    open: function () {
        this.isClose = false;
        $('.notify-dialog').show();
    },
    filePreview: function (fileId, fileName) {
        DocOnline.showFile({
            csMethod: 'Core.AttachmentClass.OpenDynamicFile',
            fileName: fileName,
            param: { fileId: fileId }
        });
    },
    unreadedCount: 0,
    getUnreadedCount: function () {
        if (this.unreadedCount)
            return this.unreadedCount;
        else
            return 0;
    },
    markReaded: function (msgId) {
        var me = this;
        ACS('CZCLZ.PM.MarkMessagesReaded', function (retVal) {
            var container = $('.notify-dialog .content');
            var msgList = $('.notify-dialog .content li');
            for (var i = 0; i < msgList.length; i++) {
                var messageId = $(msgList[i]).data('msgid');
                if (messageId == msgId)
                    $(msgList[i]).remove();
            }
            if (msgList.length < 2) {
                me.close();
            }
        }, CS.onError, [msgId]);
    }
};

//setInterval(function () {
//    ACS('CZCLZ.PM.GetInstantMassages', function (retVal) {
//        var dialogContainer = $('.notify-dialog');
//        msgLib.unreadedCount = 0;
//        if (retVal.length == 0) {
//            msgLib.close();
//            return;
//        }
//        else {
//            msgLib.unreadedCount = retVal.length;
//            if (!msgLib.isClose) {
//                msgLib.open();
//            }
//            else {
//                var msgList = $('.notify-dialog .content li');
//                if (retVal.length > 0 && msgList.length != retVal.length) {
//                    msgLib.open();
//                }
//                else {
//                    msgLib.close();
//                }
//            }
//        }

//        var msgContainer = $('.notify-dialog ul.content');
//        msgContainer.empty();
//        var html = "";
//        for (var i = 0; i < retVal.length; i++) {
//            html += '<li data-msgid="' + retVal[i].ZNXX_ID + '">'
//                + '<div class="readstatus unreaded"></div>'
//                + '<div class="msg-content-container">'
//                + '    <div class="msg-info"><span style="color:#000;">来源：</span><span>' + (retVal[i].FSR_BM_MC ? ('【' + retVal[i].FSR_BM_MC + '】') : '') + retVal[i].FSR_MC + '</span></div>'
//                + '    <div class="msg-sendtime"><span style="color:#000;">发送时间：</span>' + retVal[i].ADDTIME.toStdString(true) + '</div>'
//                + '    <div class="msg-content">标题：' + (retVal[i].ZNXX_BT ? retVal[i].ZNXX_BT : "无") + '<br/>' + Ext.String.htmlEncode(retVal[i].ZNXX_NR ? retVal[i].ZNXX_NR : "") + '</div>'
//                + '    <div class="msg-buttons"><a href="javascript:void(0);" onclick="msgLib.viewMsg(\'' + retVal[i].ZNXX_ID + '\')" style="float:right;margin-left:10px;display:none"><img src="approot/d/main/images/msg-btn-view.png" /></a>'
//                + (retVal[i].ZNXX_LX == 0 ? '' : '<a href="javascript:void(0);" onclick="msgLib.replyMsg(\'' + retVal[i].FSR_ID + '\',\'' + retVal[i].ZNXX_ID + '\')" style="float:right;"><img src="approot/d/main/images/msg-btn-reply.png" /></a>')
//                + '</div>'
//                + '</div>'
//                + '</li>';
//        }
//        msgContainer.append(html);
//    }, function () { }, 1);
//}, 10000);

