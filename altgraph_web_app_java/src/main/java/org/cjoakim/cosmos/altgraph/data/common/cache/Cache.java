package org.cjoakim.cosmos.altgraph.data.common.cache;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;
import org.cjoakim.cosmos.altgraph.data.DataAppConfiguration;
import org.cjoakim.cosmos.altgraph.data.common.DataConstants;
import org.cjoakim.cosmos.altgraph.data.common.graph.v1.TripleQueryStruct;
import org.cjoakim.cosmos.altgraph.data.common.io.FileUtil;
import org.cjoakim.cosmos.altgraph.data.common.model.npm.Library;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.StringRedisTemplate;
import org.springframework.data.redis.core.ValueOperations;
import org.springframework.stereotype.Component;

/**
 * This class implements all Caching functionality for the application.
 * It can be configured to use either local disk files, or Redis.
 * In the case of Redis, it uses a Spring Data StringRedisTemplate rather than a Repository.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Slf4j
public class Cache implements DataConstants {

    private String cacheMethod = DataAppConfiguration.getCacheMethod();
    private boolean useRedis = DataAppConfiguration.cacheUsingRedis();
    private StringRedisTemplate redisTemplate;
    private ValueOperations<String, String> redisOps;

    private FileUtil fileUtil = new FileUtil();

    @Autowired
    public Cache(StringRedisTemplate rt) {
        super();
        redisTemplate = rt;
        redisOps = redisTemplate.opsForValue();
        log.warn("Default constructor, cacheMethod: " + cacheMethod + ", redisTemplate: " + redisTemplate);
        log.warn("useRedis: " + useRedis);
    }

    public boolean isUsingRedis() {

        return useRedis;
    }

    public void toggleToRedis() {
        useRedis = true;
        log.warn("toggleToRedis, useRedis is now " + useRedis);
    }

    public void toggleToDisk() {
        useRedis = false;
        log.warn("toggleToDisk, useRedis is now " + useRedis);
    }

    public boolean putLibrary(Library lib) {
        if (useRedis) {
            String key = "library|" + lib.getName();
            try {
                redisOps.set(key, lib.asJson(false));
                log.warn("cached key to redis: " + key);
                return true;
            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }
        } else {
            String filename = getLibraryCacheFilename(lib.getName());
            try {
                fileUtil.writeJson(lib, filename, true, true);
                log.warn("cached file to disk: " + filename);
                return true;
            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }
        }
    }

    public Library getLibrary(String libName) {
        if (useRedis) {
            String key = "library|" + libName;
            log.warn("reading redis key: " + key);
            String json = redisOps.get(key);
            ObjectMapper mapper = new ObjectMapper();
            try {
                return mapper.readValue(json, Library.class);
            } catch (JsonProcessingException e) {
                e.printStackTrace();
                return null;
            }
        } else {
            try {
                String cacheFilename = getLibraryCacheFilename(libName);
                log.warn("reading cache file: " + cacheFilename);
                return fileUtil.readLibrary(cacheFilename);
            } catch (Exception e) {
                e.printStackTrace();
                return null;
            }
        }
    }

    public boolean putTriples(TripleQueryStruct struct) {
        if (useRedis) {
            String key = "triples";
            try {
                redisOps.set(key, struct.asJson(false));
                log.warn("cached key to redis: " + key);
                return true;
            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }
        } else {
            String filename = getTripleQueryStructCacheFilename();
            try {
                fileUtil.writeJson(struct, filename, true, true);
                log.warn("cached file to disk: " + filename);
                return true;
            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }
        }
    }

    public TripleQueryStruct getTriples() {
        if (useRedis) {
            String key = "triples";
            log.warn("reading redis key: " + key);
            String json = redisOps.get(key);
            ObjectMapper mapper = new ObjectMapper();
            try {
                return mapper.readValue(json, TripleQueryStruct.class);
            } catch (JsonProcessingException e) {
                e.printStackTrace();
                return null;
            }
        } else {
            try {
                String cacheFilename = getTripleQueryStructCacheFilename();
                log.warn("reading cache file: " + cacheFilename);
                return fileUtil.readTripleQueryStruct(cacheFilename);
            } catch (Exception e) {
                e.printStackTrace();
                return null;
            }
        }
    }

    private String getLibraryCacheFilename(String libName) {
        return "data/cache/" + libName + ".json";
    }

    private String getTripleQueryStructCacheFilename() {
        return "data/cache/TripleQueryStruct.json";
    }

}
