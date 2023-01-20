package org.cjoakim.cosmos.altgraph.data;

import com.azure.core.credential.AzureKeyCredential;
import com.azure.cosmos.CosmosClientBuilder;
import com.azure.cosmos.DirectConnectionConfig;
import com.azure.cosmos.GatewayConnectionConfig;
import com.azure.spring.data.cosmos.config.AbstractCosmosConfiguration;
import com.azure.spring.data.cosmos.config.CosmosConfig;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.repository.ResponseDiagnosticsProcessorImpl;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.repository.configuration.EnableRedisRepositories;

import java.util.ArrayList;

/**
 * This class is a Spring Boot @Configuration class that also provides configuration
 * values for the Spring Data Cosmos DB repositories.
 *
 * Chris Joakim, Microsoft, November 2022
 */

@Configuration
@EnableRedisRepositories(basePackages = { "org.cjoakim.cosmos.altgraph" })
@Slf4j
public class DataAppConfiguration extends AbstractCosmosConfiguration
        implements DataConstants {

    private static DataAppConfiguration singleton;

    public DataAppConfiguration() {
        super();
        log.warn("DataAppConfiguration default constructor (singleton)");
        singleton = this;
    }

    public static DataAppConfiguration getInstance() {

        return singleton;
    }

    private static String[] commandLineArgs = null;

    // Generic methods:

    protected static void setCommandLineArgs(String[] args) {

        commandLineArgs = args;
        if (commandLineArgs == null) {
            log.warn("setCommandLineArgs; null");
        }
        else {
            log.warn("setCommandLineArgs; length: " + commandLineArgs.length);
            for (int i = 0; i < commandLineArgs.length; i++) {
                log.warn("setCommandLineArgs, idx: " + i + " -> " + commandLineArgs[i]);
            }
        }
    }

    public static String[] getCommandLineArgs() {
        return commandLineArgs;
    }

    public static String flagArg(String flagArg) {

        for (int i = 0; i < commandLineArgs.length; i++) {
            if (commandLineArgs[i].equalsIgnoreCase(flagArg)) {
                return commandLineArgs[i + 1];
            }
        }
        return null;
    }

    public static String flagArg(String flagArg, String defaultValue) {

        for (int i = 0; i < commandLineArgs.length; i++) {
            if (commandLineArgs[i].equalsIgnoreCase(flagArg)) {
                return commandLineArgs[i + 1];
            }
        }
        return defaultValue;
    }

    public static boolean booleanArg(String flagArg) {

        for (int i = 0; i < commandLineArgs.length; i++) {
            if (commandLineArgs[i].equalsIgnoreCase(flagArg)) {
                return true;
            }
        }
        return false;
    }

    public static long longFlagArg(String flagArg, long defaultValue) {

        try {
            return Long.parseLong(flagArg(flagArg));
        }
        catch (NumberFormatException e) {
            return defaultValue;
        }
    }

    public static boolean isVerbose() {

        return booleanArg(VERBOSE_FLAG);
    }

    public static boolean isSilent() {

        return booleanArg(SILENT_FLAG);
    }

    public static boolean isPretty() {

        return booleanArg(PRETTY_FLAG);
    }

    // Application Config:


    public static String getTenant() {

        return flagArg(TENANT_FLAG, DEFAULT_TENANT);
    }

    public static String getCosmosContainerName() {

        return flagArg(CONTAINER_FLAG, DEFAULT_CONTAINER);
    }

    public static ArrayList<String> getPreferredRegions() {

        log.warn("getPreferredRegions: " + getInstance().preferredRegionsString);

        ArrayList<String> list = new ArrayList<String>();
        if (getInstance().preferredRegionsString != null) {
            String[] regions = getInstance().preferredRegionsString.split(",");
            for (String region: regions) {
                list.add(region);
            }
        }
        return list;
    }

    public static String getLineOfBusiness() {

        return flagArg(LOB_FLAG, LOB_NPM_LIBRARIES);
    }

    @Value("${app.cache.method}")
    public String cacheMethod;

    public static String getCacheMethod() {
        return singleton.cacheMethod;
    }

    public static boolean cacheUsingRedis() {
        if (singleton.cacheMethod == null) {
            return false;
        }
        return singleton.cacheMethod.trim().equalsIgnoreCase(CACHE_WITH_REDIS);
    }

    // Cosmos DB Spring Data Config below:
    // See https://docs.microsoft.com/en-us/azure/developer/java/spring-framework/how-to-guides-spring-data-cosmosdb

    @Value("${spring.cloud.azure.cosmos.endpoint}")
    public String uri;

    //@Value("${azure.cosmos.key}")
    @Value("${spring.cloud.azure.cosmos.key}")
    public String key;

    //@Value("${azure.cosmos.database}")
    @Value("${spring.cloud.azure.cosmos.database}")
    public String dbName;

    @Value("${azure.cosmos.regions}")
    private String preferredRegionsString;

    @Value("${azure.cosmos.queryMetricsEnabled}")
    private boolean queryMetricsEnabled;

    @Value("${azure.cosmos.maxDegreeOfParallelism}")
    public int maxDegreeOfParallelism;

    @Value("${app.imdb.graph.source}")
    public String imdbGraphSource;
    @Value("${app.imdb.graph.directed}")
    public boolean imdbGraphDirected;

    private AzureKeyCredential azureKeyCredential;

    @Bean
    public CosmosClientBuilder getCosmosClientBuilder() {

        log.warn("getCosmosClientBuilder, uri: " + uri);

        this.azureKeyCredential = new AzureKeyCredential(key);
        DirectConnectionConfig directConnectionConfig = new DirectConnectionConfig();
        GatewayConnectionConfig gatewayConnectionConfig = new GatewayConnectionConfig();
        return new CosmosClientBuilder()
                .endpoint(uri)
                .credential(azureKeyCredential)
                .directMode(directConnectionConfig, gatewayConnectionConfig);
    }

    @Override
    public CosmosConfig cosmosConfig() {

        log.warn("cosmosConfig, queryMetricsEnabled: " + queryMetricsEnabled);

        return CosmosConfig.builder()
                .responseDiagnosticsProcessor(new ResponseDiagnosticsProcessorImpl())
                .enableQueryMetrics(true)
                .build();
    }

    @Override
    protected String getDatabaseName() {

        log.warn("getDatabaseName returning: " + dbName);
        return dbName;
    }

}