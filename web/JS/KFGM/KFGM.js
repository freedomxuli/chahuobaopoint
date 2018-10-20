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

var hisStore = Ext.create('Ext.data.Store', {
    fields: [
        'PlatToSaleId', 'UserID', 'UserXM', 'points', 'discount', 'discountmemo'
    ]
})
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

function getHisSale(UserID)
{
    CS('CZCLZ.KFGMMag.getHisSale', function (retVal) {
        if (retVal)
        {
            hisStore.loadData(retVal);
        }
    }, CS.onError, UserID);
}


function kfgm(id) {
    var r = store.findRecord("PlatPointId", id).data;
    var win = new KFList({ PlatPointId: id, MaxPoint: r.Points, UserID: r.UserID });
    win.show(null,function(){
        getHisSale(r.UserID);
    });
    //var win = new addWin();
    //win.show(null, function () {
    //    Ext.getCmp("PlatPointId").setValue(id);
    //    Ext.getCmp("MaxPoint").setValue(r.Points);
    //});
}

function CXKF(PlatToSaleId, UserID, PlatPointId, MaxPoint, discount, discountmemo)
{
    var win = new addWin({ UserID: UserID, PlatToSaleId: PlatToSaleId });
    win.show(null, function () {
        Ext.getCmp("PlatPointId").setValue(PlatPointId);
        Ext.getCmp("MaxPoint").setValue(MaxPoint);
        Ext.getCmp("discount").setValue(discount);
        Ext.getCmp("discount").setDisabled(true);
        Ext.getCmp("discountmemo").setValue(discountmemo);
        Ext.getCmp("discountmemo").setDisabled(true);
    });
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('KFList', {
    extend: 'Ext.window.Window',

    height: 407,
    width: 634,
    layout: {
        type: 'fit'
    },
    title: '历史开放',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'gridpanel',
                    border: 1,
                    columnLines: 1,
                    store: hisStore,

                    columns: [
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '专线'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'points',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '线上剩余运费券'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'discountmemo',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '线上折扣'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'PlatToSaleId',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '操作',
                            renderer: function(value, cellmeta, record, rowIndex, columnIndex, store)
                            {
                                return "<a href='javascsript:void(0);' onclick='CXKF(\"" + value + "\",\"" + record.data.UserID + "\",\"" + me.PlatPointId + "\",\"" + me.MaxPoint + "\",\"" + record.data.discount + "\",\"" + record.data.discountmemo + "\");'>重新开放</a>";
                            }
                        }
                    ]
                }
            ],
            buttonAlign: 'center',
            buttons: [
                {
                    text: '新增开放折扣',
                    iconCls: 'add',
                    handler: function () {
                        var win = new addWin({ UserID: me.UserID, PlatToSaleId: "" });
                        win.show(null, function () {
                            Ext.getCmp("PlatPointId").setValue(me.PlatPointId);
                            Ext.getCmp("MaxPoint").setValue(me.MaxPoint);
                        })
                    }
                },
                {
                    text: '关闭',
                    iconCls: 'close',
                    handler: function () {
                        me.close();
                    }
                }
            ]
        });

        me.callParent(arguments);
    }

});

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 250,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '开放购买运费券',

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
                        minValue: 1,
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
                                    msg: '开放购买的运费券必须大于1',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var maxpoint = Ext.getCmp("MaxPoint").getValue();
                            if (point > maxpoint) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '开放购买的运费券不得超过其所有运费券',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                values["PlatToSaleId"] = me.PlatToSaleId;
                                var me = this;
                                CS('CZCLZ.KFGMMag.SaveKFGM', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getList(1);
                                        getHisSale(me.UserID);
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
                            text: '运费券'
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
                                                DownloadFile("CZCLZ.KFGMMag.GetKFGMToFile", "开放购买运费券.xls", Ext.getCmp("cx_yhm").getValue());
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