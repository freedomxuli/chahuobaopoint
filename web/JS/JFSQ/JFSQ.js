//

var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'sqId' },
       { name: 'userId' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'sqrq' },
       { name: 'memo' },
       { name: 'sqjf' },
       { name: 'issq' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.JFSQMag.GetList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue());
}


function sq(id) {
    if (privilege("申请积分_积分授权_编辑")){
        Ext.MessageBox.confirm('提示', '是否要授权!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.JFSQMag.JFSQ', function (retVal) {
                    if (retVal) {
                        getList(1);
                    }
                }, CS.onError, id,1);
            }
            else {
                return;
            }
        });
    }
}
//************************************页面方法***************************************

//************************************弹出界面***************************************

//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('JFSQView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'JFSQGrid',
                    store: store,
                    
                    columnLines: 1,
                    border: 1,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'UserXM',
                             sortable: false,
                             menuDisabled: true,
                             width: 400,
                             text: '物流名称'
                         },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            width: 400,
                            text: '用户名'
                        },
                        {
                            xtype: 'datecolumn',
                            dataIndex: 'sqrq',
                            sortable: false,
                            menuDisabled: true,
                            format:'Y-m-d',
                            width: 200,
                            text: '申请日期'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'sqjf',
                            sortable: false,
                            menuDisabled: true,
                            format: 'Y-m-d',
                            width: 200,
                            text: '申请积分'
                        },
                        {
                            xtype: 'datecolumn',
                            dataIndex: 'issq',
                            sortable: false,
                            menuDisabled: true,
                            format: 'Y-m-d',
                            width: 200,
                            text: '是否授权',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                if (value == 0) {
                                    str = "否";
                                } else {
                                    str = "是";
                                }
                                return str;
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'sqId',
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                str = "";
                                if (record.data.issq == 0) {
                                    str = "<a href='JavaScript:void(0)' onclick='sq(\"" + value + "\")'>授权</a>";
                                } 
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
                                    xtype: 'textfield',
                                    id: 'cx_xm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '物流名称'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_yhm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '用户名'
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'search',
                                            text: '查询',
                                            handler: function () {
                                                getList(1);
                                            }
                                        }
                                    ]
                                },
                            ]
                        }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new JFSQView();

    getList();
})
//************************************主界面*****************************************