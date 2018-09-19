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
       { name: 'PlatPointId' },
       { name: 'companyId' },
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'Points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.KFGMMag.GetList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize,  Ext.getCmp("cx_yhm").getValue());
}


function kfgm(id) {
    var r = store.findRecord("PlatPointId", id).data;
    var win = new addWin();
    win.show(null, function () {
        Ext.getCmp("PlatPointId").setValue(id);
        Ext.getCmp("MaxPoint").setValue(r.Points);
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
    title: '开放购买电子券',

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
                        id: 'PlatPointId',
                        name: 'PlatPointId',
                        hidden: true,
                    },
                    {
                        xtype: 'numberfield',
                        id: 'MaxPoint',
                        name: 'MaxPoint',
                        hidden: true,
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '分数',
                        id: 'points',
                        name: 'points',
                        allowBlank: false,
                        allowDecimals: false,
                        allowNegative: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    
                    {
                        xtype: 'numberfield',
                        fieldLabel: '优惠折扣',
                        id: 'discount',
                        name: 'discount',
                        allowDecimals : true,    //是否允许小数
                        decimalPrecision : 2,    // 精确的位数
                        allowNegative : false, 
                        minValue: 0,
                        maxValue:1,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: "label",
                        text: "*优惠折扣必须为介于0-1的两位小数！",
                        style:"color:red;padding-left:100px;"
                    },
                    {
                        xtype: 'textareafield',
                        id: 'discountmemo',
                        name: 'discountmemo',
                        fieldLabel: '优惠折扣备注',
                        allowBlank: false,
                        anchor: '100%'
                    },
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var point = Ext.getCmp("points").getValue();
                            if (point < 0) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '开放购买的电子券必须大于0',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var maxpoint = Ext.getCmp("MaxPoint").getValue();
                            if (point > maxpoint) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '开放购买的电子券不得超过其所有电子券',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.KFGMMag.SaveKFGM', function (retVal) {
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
                            dataIndex: 'Points',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '电子券'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'PlatPointId',
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                str = "<a href='JavaScript:void(0)' onclick='kfgm(\"" + value + "\")'>开放购买</a>";
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
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            text: '导出',
                                            iconCls: 'download',
                                            handler: function () {
                                                DownloadFile("CZCLZ.KFGMMag.GetKFGMToFile", "开放购买电子券.xls", Ext.getCmp("cx_yhm").getValue());
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