var MenuStore = Ext.create('Ext.data.TreeStore', {
    fields: [
        'ML_ID', 'ML_MC', 'ML_LB', 'ML_PX', 'ML_URL', 'MODULE_ID', 'MENU_ID'
    ]
});

Ext.onReady(function () {
    Ext.define('MainView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;

            Ext.applyIf(me, {
                items: [
                    {
                        xtype: 'treepanel',
                        height: 250,
                        width: 400,
                        store: MenuStore,
                        viewConfig: {

                        },
                        columnLines: 1,
                        border: 1,
                        columns: [
                            {
                                xtype: 'treecolumn',
                                dataIndex: 'ML_MC',
                                text: '名称',
                                sortable: false,
                                menuDisabled: true,
                                width: 300
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ML_LB',
                                sortable: false,
                                menuDisabled: true,
                                text: '类别',
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str = "总目录";
                                    switch (value)
                                    {
                                        case 0:
                                            str = "模块";
                                            break;
                                        case 1:
                                            str = "菜单";
                                            break;
                                        case 2:
                                            str = "权限";
                                            break;
                                    }
                                    return str;
                                }
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ML_PX',
                                sortable: false,
                                menuDisabled: true,
                                text: '序号'
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ML_ID',
                                sortable: false,
                                menuDisabled: true,
                                text: '操作',
                                width: 300,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    if (!value) {
                                        return "<span style='color:blue;cursor:pointer;' onClick='EditMK(\"" + value + "\",\"" + record.data.ML_MC + "\",\"" + record.data.ML_PX + "\");'>新增模块</span>";
                                    } else {
                                        var str = "";
                                        switch (record.data.ML_LB) {
                                            case 0:
                                                str = "<span style='color:blue;cursor:pointer;' onClick='EditMenu(\"\",\"" + record.data.MODULE_ID + "\",\"" + record.data.ML_MC + "\",\"" + record.data.ML_PX + "\",\"" + record.data.ML_URL + "\");'>新增菜单</span>　<span style='color:blue;cursor:pointer;' onClick='EditMK(\"" + value + "\",\"" + record.data.ML_MC + "\",\"" + record.data.ML_PX + "\");'>修改模块</span>　<span style='color:blue;cursor:pointer;' onClick='DeleteMK(\"" + value + "\")'>删除模块</span>";
                                                break;
                                            case 1:
                                                str = "<span style='color:blue;cursor:pointer;' onClick='EditPrivilege(\"\",\"" + record.data.MENU_ID + "\",\"" + record.data.MODULE_ID + "\",\"" + record.data.ML_MC + "\",\"" + record.data.ML_PX + "\");'>新增权限</span>　<span style='color:blue;cursor:pointer;' onClick='EditMenu(\"" + value + "\",\"" + record.data.MODULE_ID + "\",\"" + record.data.ML_MC + "\",\"" + record.data.ML_PX + "\",\"" + record.data.ML_URL + "\");'>修改菜单</span>　<span style='color:blue;cursor:pointer;' onClick='DeleteMenu(\"" + value + "\");'>删除菜单</span>";
                                                break;
                                            case 2:
                                                str = "<span style='color:blue;cursor:pointer;' onClick='EditPrivilege(\"" + value + "\",\"" + record.data.MENU_ID + "\",\"" + record.data.MODULE_ID + "\",\"" + record.data.ML_MC + "\",\"" + record.data.ML_PX + "\");'>修改权限</span>　<span style='color:blue;cursor:pointer;' onClick='DeletePrivilege(\"" + value + "\");'>删除权限</span>";
                                                break;
                                        }
                                        return str;
                                    }
                                }
                            }
                        ]
                    }
                ]
            });

            me.callParent(arguments);
        }

    });

    new MainView();

    dataBind();
});

function dataBind()
{
    CS("CZCLZ.Module.GetModuleTree", function (ret) {
        MenuStore.setRootNode(ret);
    }, CS.onError, "");
}

Ext.define('MKWindow', {
    extend: 'Ext.window.Window',

    height: 252,
    width: 485,
    layout: {
        type: 'fit'
    },
    title: '模块编辑',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'form',
                    id: 'EditMKform',
                    items: [
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'moduleId',
                            name: 'moduleId',
                            hidden: true,
                            fieldLabel: '模块ID'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'moduleName',
                            name: 'moduleName',
                            allowblank: true,
                            fieldLabel: '模块名称'
                        },
                        {
                            xtype: 'numberfield',
                            padding: 10,
                            width: 455,
                            minValue: 0,
                            value: 0,
                            id: 'modulePx',
                            name: 'modulePx',
                            allowblank: true,
                            fieldLabel: '模块排序'
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '保存',
                            iconCls: 'save',
                            handler: function () {
                                var form = Ext.getCmp('EditMKform');
                                if (form.form.isValid()) {
                                    var values = form.form.getValues(false);
                                    CS('CZCLZ.Module.SaveModule', function (retVal) {
                                        if (retVal)
                                        {
                                            Ext.Msg.alert("提示", "模块保存成功！", function () {
                                                dataBind();
                                                me.close();
                                            });
                                        }
                                    }, CS.onError, values);
                                }
                            }
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});

function EditMK(moduleId, moduleName, modulePx)
{
    var win = new MKWindow();
    win.show(null, function () {
        if (moduleId)
        {
            Ext.getCmp("moduleId").setValue(moduleId);
            Ext.getCmp("moduleName").setValue(moduleName);
            Ext.getCmp("modulePx").setValue(modulePx);
        }
    });
}

function DeleteMK(moduleId)
{
    CS('CZCLZ.Module.DeleteModule', function (retVal) {
        if (retVal)
        {
            Ext.Msg.alert("提示", "模块删除成功！", function () {
                dataBind();
            });
        }
    }, CS.onError, moduleId)
}

Ext.define('MenuWindow', {
    extend: 'Ext.window.Window',

    height: 252,
    width: 485,
    layout: {
        type: 'fit'
    },
    title: '菜单编辑',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'form',
                    id: 'EditMenuform',
                    items: [
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'menuId',
                            name: 'menuId',
                            hidden: true,
                            fieldLabel: '菜单ID'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'moduleId',
                            name: 'moduleId',
                            hidden: true,
                            fieldLabel: '模块ID'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'menuName',
                            name: 'menuName',
                            allowblank: true,
                            fieldLabel: '菜单名称'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'menuurl',
                            name: 'menuurl',
                            allowblank: true,
                            fieldLabel: '菜单链接'
                        },
                        {
                            xtype: 'numberfield',
                            padding: 10,
                            width: 455,
                            minValue: 0,
                            value: 0,
                            id: 'menuPx',
                            name: 'menuPx',
                            allowblank: true,
                            fieldLabel: '菜单排序'
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '保存',
                            iconCls: 'save',
                            handler: function () {
                                var form = Ext.getCmp('EditMenuform');
                                if (form.form.isValid()) {
                                    var values = form.form.getValues(false);
                                    CS('CZCLZ.Module.SaveMenu', function (retVal) {
                                        if (retVal) {
                                            Ext.Msg.alert("提示", "菜单保存成功！", function () {
                                                dataBind();
                                                me.close();
                                            });
                                        }
                                    }, CS.onError, values);
                                }
                            }
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});

function EditMenu(menuId, moduleId, menuName, menuPx, menuurl)
{
    var win = new MenuWindow();
    win.show(null, function () {
        if (menuId) {
            Ext.getCmp("moduleId").setValue(moduleId);
            Ext.getCmp("menuId").setValue(menuId);
            Ext.getCmp("menuName").setValue(menuName);
            Ext.getCmp("menuurl").setValue(menuurl);
            Ext.getCmp("menuPx").setValue(menuPx);
        } else {
            Ext.getCmp("moduleId").setValue(moduleId);
        }
    });
}

function DeleteMenu(menuId) {
    CS('CZCLZ.Module.DeleteMenu', function (retVal) {
        if (retVal) {
            Ext.Msg.alert("提示", "菜单删除成功！", function () {
                dataBind();
            });
        }
    }, CS.onError, menuId)
}

Ext.define('PrivilegeWindow', {
    extend: 'Ext.window.Window',

    height: 252,
    width: 485,
    layout: {
        type: 'fit'
    },
    title: '菜单编辑',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'form',
                    id: 'EditPrivilegeform',
                    items: [
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'privilegeId',
                            name: 'privilegeId',
                            hidden: true,
                            fieldLabel: '权限ID'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'menuId',
                            name: 'menuId',
                            hidden: true,
                            fieldLabel: '菜单ID'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'moduleId',
                            name: 'moduleId',
                            hidden: true,
                            fieldLabel: '模块ID'
                        },
                        {
                            xtype: 'textfield',
                            padding: 10,
                            width: 455,
                            id: 'privilegeName',
                            name: 'privilegeName',
                            allowblank: true,
                            fieldLabel: '权限名称'
                        },
                        {
                            xtype: 'numberfield',
                            padding: 10,
                            width: 455,
                            minValue: 0,
                            value: 0,
                            id: 'privilegePx',
                            name: 'privilegePx',
                            allowblank: true,
                            fieldLabel: '权限排序'
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '保存',
                            iconCls: 'save',
                            handler: function () {
                                var form = Ext.getCmp('EditPrivilegeform');
                                if (form.form.isValid()) {
                                    var values = form.form.getValues(false);
                                    CS('CZCLZ.Module.SavePrivilege', function (retVal) {
                                        if (retVal) {
                                            Ext.Msg.alert("提示", "权限保存成功！", function () {
                                                dataBind();
                                                me.close();
                                            });
                                        }
                                    }, CS.onError, values);
                                }
                            }
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});

function EditPrivilege(privilegeId, menuId, moduleId, privilegeName, privilegePx) {
    var win = new PrivilegeWindow();
    win.show(null, function () {
        if (privilegeId) {
            Ext.getCmp("privilegeId").setValue(privilegeId);
            Ext.getCmp("moduleId").setValue(moduleId);
            Ext.getCmp("menuId").setValue(menuId);
            Ext.getCmp("privilegeName").setValue(privilegeName);
            Ext.getCmp("privilegePx").setValue(privilegePx);
        } else {
            Ext.getCmp("moduleId").setValue(moduleId);
            Ext.getCmp("menuId").setValue(menuId);
        }
    });
}

function DeletePrivilege(privilegeId) {
    CS('CZCLZ.Module.DeletePrivilege', function (retVal) {
        if (retVal) {
            Ext.Msg.alert("提示", "权限删除成功！", function () {
                dataBind();
            });
        }
    }, CS.onError, privilegeId)
}