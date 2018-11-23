var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'PlatToSaleId' },
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.XJMag.GetList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue());
}

function xj(id) {
    var r = store.findRecord("PlatToSaleId", id).data;
    CS('CZCLZ.KFGMMag.getHisSale', function (retVal) {

        var win = new addWin();
        win.show(null, function () {
            Ext.getCmp("PlatToSaleId").setValue(id);
            Ext.getCmp("MaxPoint").setValue(r.points);

            if (retVal) {
                var str = "线上剩余运费券:" + retVal[0]["points"] + " 线上折扣:" + retVal[0]["discountmemo"];
                Ext.getCmp("zspoint").setText(str);
            }
        });
    }, CS.onError, r.UserID);
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
    title: '下架运费券',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'UserId',
                        name: 'UserId',
                        hidden: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'PlatToSaleId',
                        name: 'PlatToSaleId',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        id: 'MaxPoint',
                        name: 'MaxPoint',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '运费券',
                        id: 'points',
                        name: 'points',
                        allowBlank: false,
                        allowDecimals: false,
                        allowNegative: false,
                        minValue: 1,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textareafield',
                        id: 'memo',
                        name: 'memo',
                        fieldLabel: '下架备注',
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: "label",
                        id: "zspoint",
                        text: "",
                        style: "padding-left:100px;"
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var point = Ext.getCmp("points").getValue();
                            if (point < 1) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '下架的运费券必须大于1',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var maxpoint = Ext.getCmp("MaxPoint").getValue();
                            if (point > maxpoint) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '下架的运费券不得超过其线上所有运费券',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.XJMag.SaveXJ', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getList(1);
                                    }
                                }, CS.onError, values);

                            }
                        }
                    },
                     {
                         text: '取消',
                         iconCls: 'back',
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
    Ext.define('KFGMView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'KFGMGrid',
                    store: store,
                    columnLines: 1,
                    border: 1,
                    columns: [
                        {
                            xtype: 'rownumberer',
                            width:30
                        },
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
                            dataIndex: 'points',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '线上运费券'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'PlatToSaleId',
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (record.data.points > 0)
                                {
                                    str = "<a href='JavaScript:void(0)' onclick='xj(\"" + value + "\")'>下架</a>";
                                    return str;
                                }
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
                                    id: 'cx_yhm',
                                    labelWidth: 60,
                                    fieldLabel: '物流名称'
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

    new KFGMView();

    getList();
})
//************************************主界面*****************************************