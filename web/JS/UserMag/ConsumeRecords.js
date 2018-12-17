var pageSize = 15;

var orderstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'KIND' },
        { name: 'UserName' },
        { name: 'UserXM' },
        { name: 'DATE' },
        { name: 'MONEY' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getConsumeListByZX(nPage);
    }
});

var orderstore1 = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'KIND' },
        { name: 'UserName' },
        { name: 'UserXM' },
        { name: 'DATE' },
        { name: 'MONEY' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getConsumeListBySF(nPage);
    }
});

var KindStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'ID' },
       { name: 'MC' }
    ],
    data: [
        {
            'ID': '',
            'MC': '全部'
        },
        {
            'ID': '1',
            'MC': '申请'
        },
        {
            'ID': '2',
            'MC': '支付'
        },
        {
            'ID': '3',
            'MC': '收券'
        }
    ]
});

var KindStore1 = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'ID' },
       { name: 'MC' }
    ],
    data: [
        {
            'ID': '',
            'MC': '全部'
        },
        {
            'ID': '1',
            'MC': '购买'
        },
        {
            'ID': '2',
            'MC': '消费'
        },
        {
            'ID': '3',
            'MC': '转让'
        }
    ]
});


Ext.onReady(function () {
    Ext.define('ConsumeView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;

            Ext.applyIf(me, {
                items: [
                    {
                        xtype: 'tabpanel',
                        activeTab: 0,
                        items: [
                            {
                                xtype: 'panel',
                                layout: {
                                    type: 'fit'
                                },
                                title: '专线记录',
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        border: 1,
                                        columnLines: 1,
                                        store: orderstore,
                                        columns: [
                                            {
                                                xtype: 'gridcolumn',
                                                dataIndex: 'KIND',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '类别'
                                            },
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
                                                dataIndex: 'UserName',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '交易对象'
                                            },
                                            {
                                                xtype: 'datecolumn',
                                                dataIndex: 'DATE',
                                                format: 'Y-m-d',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '交易时间'
                                            },
                                            {
                                                xtype: 'gridcolumn',
                                                dataIndex: 'MONEY',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '交易金额'
                                            }
                                        ],
                                        dockedItems: [
                                            {
                                                xtype: 'toolbar',
                                                dock: 'top',
                                                items: [
                                                    {
                                                        xtype: 'combobox',
                                                        id: 'cx_kind_zx',
                                                        width: 160,
                                                        fieldLabel: '类型',
                                                        editable: false,
                                                        labelWidth: 40,
                                                        store: KindStore,
                                                        queryMode: 'local',
                                                        displayField: 'MC',
                                                        valueField: 'ID',
                                                        value: ''
                                                    },
                                                    {
                                                        id: 'cx_beg_zx',
                                                        xtype: 'datefield',
                                                        fieldLabel: '交易时间段',
                                                        format: 'Y-m-d',
                                                        labelWidth: 80,
                                                        width: 210
                                                    },
                                                    {
                                                        id: 'cx_end_zx',
                                                        xtype: 'datefield',
                                                        format: 'Y-m-d',
                                                        fieldLabel: '至',
                                                        labelWidth: 20,
                                                        width: 150
                                                    },
                                                    {
                                                        xtype: 'textfield',
                                                        id: 'cx_mc_zx',
                                                        width: 160,
                                                        labelWidth: 70,
                                                        fieldLabel: '专线'
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'search',
                                                        text: '查询',
                                                        handler: function () {
                                                            getConsumeListByZX(1);
                                                        }
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'view',
                                                        text: '导出记录',
                                                        handler: function () {
                                                            DownloadFile("CZCLZ.YHGLClass.GetZXXFToFile", "专线用户消费统计表.xls", Ext.getCmp("cx_kind_zx").getValue(), Ext.getCmp("cx_beg_zx").getValue(), Ext.getCmp("cx_end_zx").getValue(), Ext.getCmp("cx_mc_zx").getValue());
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                dock: 'bottom',
                                                width: 360,
                                                store: orderstore,
                                                displayInfo: true
                                            }
                                        ]
                                    }
                                ]
                            },
                            {
                                xtype: 'panel',
                                layout: {
                                    type: 'fit'
                                },
                                title: '三方记录',
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        border: 1,
                                        columnLines: 1,
                                        store: orderstore1,
                                        columns: [
                                            {
                                                xtype: 'gridcolumn',
                                                dataIndex: 'KIND',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '类别'
                                            },
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
                                                dataIndex: 'UserName',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '三方'
                                            },
                                            {
                                                xtype: 'datecolumn',
                                                dataIndex: 'DATE',
                                                format: 'Y-m-d',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '交易时间'
                                            },
                                            {
                                                xtype: 'gridcolumn',
                                                dataIndex: 'MONEY',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '交易金额'
                                            }
                                        ],
                                        dockedItems: [
                                            {
                                                xtype: 'toolbar',
                                                dock: 'top',
                                                items: [
                                                    {
                                                        xtype: 'combobox',
                                                        id: 'cx_kind_sf',
                                                        width: 160,
                                                        fieldLabel: '类型',
                                                        editable: false,
                                                        labelWidth: 40,
                                                        store: KindStore1,
                                                        queryMode: 'local',
                                                        displayField: 'MC',
                                                        valueField: 'ID',
                                                        value: ''
                                                    },
                                                    {
                                                        id: 'cx_beg_sf',
                                                        xtype: 'datefield',
                                                        fieldLabel: '交易时间段',
                                                        format: 'Y-m-d',
                                                        labelWidth: 80,
                                                        width: 210
                                                    },
                                                    {
                                                        id: 'cx_end_sf',
                                                        xtype: 'datefield',
                                                        format: 'Y-m-d',
                                                        fieldLabel: '至',
                                                        labelWidth: 20,
                                                        width: 150
                                                    },
                                                    {
                                                        xtype: 'textfield',
                                                        id: 'cx_mc_sf',
                                                        width: 160,
                                                        labelWidth: 70,
                                                        fieldLabel: '三方'
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'search',
                                                        text: '查询',
                                                        handler: function () {
                                                            getConsumeListBySF(1);
                                                        }
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'view',
                                                        text: '导出记录',
                                                        handler: function () {
                                                            DownloadFile("CZCLZ.YHGLClass.GetSFXFToFile", "三方用户消费统计表.xls", Ext.getCmp("cx_kind_sf").getValue(), Ext.getCmp("cx_beg_sf").getValue(), Ext.getCmp("cx_end_sf").getValue(), Ext.getCmp("cx_mc_sf").getValue());
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                dock: 'bottom',
                                                width: 360,
                                                store: orderstore1,
                                                displayInfo: true
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            });

            me.callParent(arguments);
        }

    });

    new ConsumeView();

    getConsumeListByZX(1);
    getConsumeListBySF(1);

});

function getConsumeListByZX(nPage)
{
    CS('CZCLZ.YHGLClass.GetOrderListByZX', function (retVal) {
        orderstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_kind_zx").getValue(), Ext.getCmp("cx_beg_zx").getValue(), Ext.getCmp("cx_end_zx").getValue(), Ext.getCmp("cx_mc_zx").getValue());
}

function getConsumeListBySF(nPage) {
    CS('CZCLZ.YHGLClass.GetOrderListBySF', function (retVal) {
        orderstore1.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_kind_sf").getValue(), Ext.getCmp("cx_beg_sf").getValue(), Ext.getCmp("cx_end_sf").getValue(), Ext.getCmp("cx_mc_sf").getValue());
}