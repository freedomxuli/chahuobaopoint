var pageSize=15;
var cx_role;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'Password' },
       { name: 'roleName' },
       { name: 'UserXM' },
       { name: 'UserTel' }
    ],
    onPageChange: function(sto, nPage, sorters) {
        getUser(nPage);
    }
});


var JsStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'roleId' },
       { name: 'roleName' }
    ]
});



//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.YHGLClass.GetUserList', function(retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_role").getValue(), Ext.getCmp("cx_yhm").getValue());
}
//************************************页面方法***************************************

//************************************弹出界面***************************************

//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function() {
    Ext.define('YhView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function() {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'usergrid',
                    title: '',
                    store: store,
                    selModel: Ext.create('Ext.selection.CheckboxModel', {

                }),
                columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            text: "登录名"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'roleName',
                            sortable: false,
                            menuDisabled: true,
                            text: "角色"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            text: "姓名"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserTel',
                            sortable: false,
                            menuDisabled: true,
                            text: "电话"
                        },
                        {
                            text: '操作',
                            dataIndex: 'UserID',
                            width:120,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function(value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                str = "<span onclick='EditUser(\"" + value + "\");'>修改</span>";
                                return str;
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
                                    xtype: 'combobox',
                                    id: 'cx_js',
                                    width: 160,
                                    fieldLabel: '角色',
                                    editable: false,
                                    labelWidth: 40,
                                    store: JsStore,
                                    queryMode: 'local',
                                    displayField: 'JS_NAME',
                                    valueField: 'JS_ID',
                                    value: ''
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_yhm',
                                    width: 140,
                                    labelWidth: 50,
                                    fieldLabel: '用户名'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_xm',
                                    width: 140,
                                    labelWidth: 50,
                                    fieldLabel: '联系人'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_dm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '所属单位'
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'search',
                                            text: '查询',
                                            handler: function() {
                                                getUser(1);
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
                                            iconCls: 'add',
                                            text: '新增',
                                            handler: function() {
                                                window.location.href = "AddUser.html";
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
                                            handler: function() {
                                                var idlist = [];
                                                var grid = Ext.getCmp("usergrid");
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

                                                Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function(obj) {
                                                    if (obj == "yes") {
                                                        for (var n = 0, len = rds.length; n < len; n++) {
                                                            var rd = rds[n];

                                                            idlist.push(rd.get("User_ID"));
                                                        }

                                                        CS('CZCLZ.YHGLClass.DelUserByids', function(retVal) {
                                                        if (retVal) {
                                                                getUser(cx_js, cx_yhm, cx_xm, cx_dm,store.currentPage);
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
                        },
                        {
                            xtype: 'pagingtoolbar',
                            displayInfo: true,
                            store: store,
                            dock: 'bottom'
                        }
                    ]
        }
            ];
            me.callParent(arguments);
        }
    });

    new YhView();

    CS('CZCLZ.YHGLClass.GetJs', function(retVal) {
        if (retVal) {
            JsStore.add([{ 'JS_ID': '', 'JS_NAME': '全部角色'}]);
            JsStore.loadData(retVal, true);
            Ext.getCmp("cx_js").setValue('');
        }
    }, CS.onError, "");

    cx_js = Ext.getCmp("cx_js").getValue();
    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_dm = Ext.getCmp("cx_dm").getValue();

    getUser(1);

})
//************************************主界面*****************************************