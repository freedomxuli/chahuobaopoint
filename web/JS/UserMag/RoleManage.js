//************************************数据源*****************************************
var RoleStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'roleId' },
       { name: 'roleName' },
       { name: 'companyId' },
       { name: 'rolePx' }
    ]
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function GetRole() {
    CS('CZCLZ.JsGlClass.GetRole', function (retVal) {
        if (retVal) {
            RoleStore.loadData(retVal);
        }
    }, CS.onError);
}


function xg(id) {
    var r = RoleStore.findRecord("roleId", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("角色修改");
        var form = Ext.getCmp('addform');
        form.form.setValues(r);
    });
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 250,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '新增角色',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                frame: true,
                bodyPadding: 10,
                title: '',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '角色ID',
                        id: 'roleId',
                        name: 'roleId',
                        labelWidth: 70,
                        hidden: true,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textareafield',
                        id: 'roleName',
                        name: 'roleName',
                        fieldLabel: '名称',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        id: 'rolePx',
                        name: 'rolePx',
                        fieldLabel: '排序',
                        labelWidth: 70,
                        value: 0,
                        allowBlank: false,
                        minValue:0,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        handler: function () {
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                //取得表单中的内容
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.JsGlClass.SaveRole', function (retVal) {
                                    if (retVal) {
                                        RoleStore.loadData(retVal);
                                    }
                                    me.up('window').close()
                                }, CS.onError, values);
                            }
                        }
                    },
                    {
                        text: '取消',
                        handler: function () {
                            this.up('window').close();
                        }
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('JsView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'JsGrid',
                    store: RoleStore,
                    selModel: Ext.create('Ext.selection.CheckboxModel', {

                    }),
                    columnLines: 1,
                    border: 1,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'roleName',
                            sortable: false,
                            menuDisabled: true,
                            width: 400,
                            text: '角色名称'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'rolePx',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '排序'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var r = record.data;
                                var id = r["roleId"];
                                return "<a href='JavaScript:void(0)' onclick='xg(\"" + id + "\")'>修改</a>";
                            }
                        }
                    ],
                    viewConfig: {

                    },
                    dockedItems: [
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'add',
                                            text: '新增',
                                            handler: function () {
                                                var win = new addWin();
                                                win.show();
                                            }
                                        }
                                    ]
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'delete',
                                            text: '删除',
                                            handler: function () {
                                                var idlist = [];
                                                var grid = Ext.getCmp("JsGrid");
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

                                                Ext.MessageBox.confirm("提示", "是否删除你所选?", function (obj) {
                                                    if (obj == "yes") {
                                                        for (var n = 0, len = rds.length; n < len; n++) {
                                                            var rd = rds[n];

                                                            idlist.push(rd.get("roleId"));
                                                        }

                                                        CS('CZCLZ.JsGlClass.DeleteRole', function (retVal) {
                                                            if (retVal) {
                                                                RoleStore.loadData(retVal);
                                                            }
                                                        }, CS.onError, idlist);
                                                    }
                                                    else {
                                                        return;
                                                    }
                                                });


                                            }
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new JsView();

    GetRole();
})
//************************************主界面*****************************************