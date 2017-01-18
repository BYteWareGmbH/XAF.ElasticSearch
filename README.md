# XAF.ElasticSearch
Integrates ElasticSearch into a XAF XPO application.
Just add the module, create classes that implement the IElasticSearchIndex and IElasticSearchIndexRefresh interfaces (examples are in MainDemo.Module), set the to be used ElasticSearch nodes in the applications config file and decorate your to be indexed Properties and Classes with the ElasticProperty and ElasticSearch attributes and/or use the appropriate model settings and the built in Full Text search uses ElasticSearch.

For further help just take a look at the included MainDemo (copied from the DevExpress MainDemo example).

You need a DevExpress Universal license and version 16.2.3 of it installed. A demo version can be downloaded from the DevExpress website.
