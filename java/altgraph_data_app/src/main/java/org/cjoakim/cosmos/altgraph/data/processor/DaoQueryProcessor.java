package org.cjoakim.cosmos.altgraph.data.processor;

import lombok.NoArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.dao.CosmosSynchDao;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.TripleQueryStruct;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

/**
 * Instances of this class are used for ad-hoc testing and development
 * of Data Access Object (Cosmos DB Java SDK) functionality within the
 * (batch) DataCommandLineApp,
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@NoArgsConstructor
@Slf4j
public class DaoQueryProcessor extends AbstractConsoleAppProcess {

    @Value("${spring.cloud.azure.cosmos.endpoint}")
    public String uri;

    //@Value("${azure.cosmos.key}")
    @Value("${spring.cloud.azure.cosmos.key}")
    public String key;

    @Value("${spring.cloud.azure.cosmos.database}")
    private String dbName;

    private CosmosSynchDao dao = new CosmosSynchDao();

    public void process() throws Exception {

        try {
            String tenant = DataAppConfiguration.getTenant();
            log.warn("process, tenant: " + tenant);
            log.warn("process, uri:    " + uri);

            dao.initialize(uri, key, dbName);

            StringBuffer sb = new StringBuffer();
            sb.append("select * from c where c.doctype = 'triple'");

            TripleQueryStruct allTriplesStruct = new TripleQueryStruct();
            allTriplesStruct.setContainerName("altgraph");
            allTriplesStruct.setSql(sb.toString());
            TripleQueryStruct result = dao.queryTriples(allTriplesStruct);
            log.warn("result.getRequestCharge(): " + result.getRequestCharge());

            FileUtil fu = new FileUtil();
            fu.writeJson(result, TRIPLE_QUERY_STRUCT_FILE, true, true);

            TripleQueryStruct struct2 = fu.readTripleQueryStruct(TRIPLE_QUERY_STRUCT_FILE);
            //log.warn(struct2.asJson(true));
            log.warn("struct type:    " + struct2.getStructType());
            log.warn("documents size: " + struct2.getDocuments().size());

//            09:44:08.197 [main] WARN  o.c.c.altgraph.data.dao.CosmosSynchDao - pageNum: 1, page.getRequestCharge(): 170.28
//            09:44:08.894 [main] WARN  o.c.c.altgraph.data.dao.CosmosSynchDao - pageNum: 2, page.getRequestCharge(): 149.6
//            09:44:08.895 [main] WARN  o.c.c.a.d.p.DaoQueryProcessor - result.getRequestCharge(): 319.88
        } finally {
            dao.close();
        }
    }
}
