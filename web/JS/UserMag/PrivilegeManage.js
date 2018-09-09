var roleStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'roleId' },
       { name: 'roleName' }
    ]
});

var MenuStore = Ext.create('Ext.data.TreeStore', {
    fields: [
        'ML_ID', 'ML_MC', 'ML_LB', 'ML_PX', 'ML_URL', 'MODULE_ID', 'MENU_ID'
    ]
});

var roleId;

Ext.onReady(function () {

    /** 递归遍历父节点 **/
    var travelParentChecked = function (node, checkStatus, opts) {
        //父节点
        var upNode = node.parentNode;
        if (upNode != null) {
            var opts = {};
            opts["isPassive"] = true;
            //父节点当前选中状态
            var upChecked = upNode.data.checked;

            //选中状态，遍历父节点，判断有父节点下的子节点是否都全选
            if (checkStatus) {
                var allChecked = true;
                //此时父节点不可能是选中状态
                //如果有一个节点未选中，可以判断，当前父节点肯定是未选中状态，所以此时不必向上遍历
                upNode.eachChild(function (child) {
                    if (!child.data.checked) {
                        allChecked = false;

                        return false;
                    }
                });

                upNode.set('checked', allChecked);
                if (allChecked) {
                    travelParentChecked(upNode, allChecked, opts);
                } else {//如果后台传递数据时，选择状态正确的话，此处不需要执行
                    //travelParentChecked(upNode, allChecked, opts);
                }
            } else {//未选中，让父节点全都 不选
                if (upNode.data.checked) {
                    upNode.set('checked', checkStatus);
                    travelParentChecked(upNode, checkStatus, opts);
                } else {
                    //travelParentChecked(upNode, allChecked, opts);
                }
            }
        }
    }

    /** 递归遍历子节点，复选框 **/
    var travelChildrenChecked = function (node, checkStatus, eOpts) {
        var isLeaf = node.data.leaf;
        if (!isLeaf) {
            node.expand(false, function () {
                if (eOpts["isPassive"] == null) {//主动点击
                    node.eachChild(function (child) {
                        child.set('checked', checkStatus);

                        travelChildrenChecked(child, checkStatus, eOpts);
                        //child.fireEvent('checkchange',child, checked);//不知什么原因，不起作用
                    });
                }
            });
        }
        node.set('checked', checkStatus);
    }
    
    Ext.define('PrivilegeWindow', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;

            Ext.applyIf(me, {
                items: [
                    {
                        xtype: 'panel',
                        layout: {
                            align: 'stretch',
                            type: 'hbox'
                        },
                        items: [
                            {
                                xtype: 'gridpanel',
                                width: 300,
                                title: '选择角色',
                                store: roleStore,
                                columns: [
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'roleName',
                                        width: 298,
                                        sortable: false,
                                        menuDisabled: true,
                                        text: '角色名称'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'roleId',
                                        width: 298,
                                        sortable: false,
                                        menuDisabled: true,
                                        hidden: true,
                                        text: '角色ID'
                                    }
                                ],
                                listeners: {
                                    itemclick: function (value, record, item, index, e, eOpts) {
                                        dataPrivilege(record.data.roleId);
                                    }
                                }
                            },
                            {
                                xtype: 'treepanel',
                                flex: 3,
                                title: '权限',
                                store: MenuStore,
                                viewConfig: {

                                },
                                id: 'PrivilegeTree',
                                rootVisible: false,
                                listeners: {
                                    //选择父节点勾选所有子节点；勾除所有子节点取出父节点勾选
                                    checkchange: function (node, checked, eOpts) {
                                        travelChildrenChecked(node, checked, eOpts);
                                        travelParentChecked(node, checked, eOpts);
                                    }
                                },
                                columns: [
                                    {
                                        xtype: 'treecolumn',
                                        dataIndex: 'ML_MC',
                                        text: '名称',
                                        width: 600,
                                        sortable: false,
                                        menuDisabled: true
                                    }
                                ]
                            }
                        ],
                        buttonAlign: 'center',
                        buttons: [
                            {
                                text: '保存',
                                iconCls: 'save',
                                handler: function () {
                                    var tree = Ext.getCmp('PrivilegeTree');
                                    var rds = tree.getChecked();
                                    var objs = [];
                                    for (var i = 0; i < rds.length; i++)
                                    {
                                        if (rds[i].data.ML_LB == 2) {
                                            if (rds[i].data.checked)
                                                objs.push(rds[i].data.ML_ID);
                                        }
                                    }
                                    if (objs.length == 0)
                                    {
                                        Ext.Msg.alert("提示", "请先勾选权限！");
                                        return;
                                    }
                                    CS('CZCLZ.Module.SavePrivilegeByRole', function (retVal) {
                                        if (retVal)
                                        {
                                            dataPrivilege(roleId);
                                        }
                                    }, CS.onError, objs, roleId);
                                }
                            }
                        ]
                    }
                ]
            });

            me.callParent(arguments);
        }

    });

    new PrivilegeWindow();

    dataRole();
});

function dataRole()
{
    CS('CZCLZ.JsGlClass.GetRole', function (retVal) {
        if (retVal) {
            roleStore.loadData(retVal);
            dataPrivilege(retVal[0]["roleId"]);
        }
    }, CS.onError);
}

function dataPrivilege(id)
{
    roleId = id;
    CS('CZCLZ.Module.GetPrivilegeByRole', function (retVal) {
        if (retVal)
        {
            MenuStore.setRootNode(retVal);
        }
    }, CS.onError, roleId)
}