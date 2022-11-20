package org.cjoakim.cosmos.altgraph.data.processor;

import io.lettuce.core.RedisClient;
import io.lettuce.core.api.StatefulRedisConnection;
import io.lettuce.core.api.sync.RedisCommands;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.List;
import java.util.Random;

/**
 * This is ad-hoc code NOT directly related to AltGraph
 * Chris Joakim, Microsoft, November 2022
 */

@Component
@Slf4j
public class RedisPocProcessor extends AbstractConsoleAppProcess {

    private RedisClient client = null;

    public void process() throws Exception {

        RedisClient client = RedisClient.create("redis://localhost:6379/");
        StatefulRedisConnection<String, String> connection = client.connect();
        RedisCommands<String, String> syncCommands = connection.sync();
        log.warn("RedisClient: " + client.toString());

        syncCommands.incr("test");
        syncCommands.get("test");
        String initialCounter = syncCommands.get("test");
        log.warn("initialCounter:        " + initialCounter);

        Random rnd = new Random();
        String chars = "abcxyz";

        long count = 5000;
        long startMs = System.currentTimeMillis();
        log.warn("populate loop count:    " + count);
        log.warn("populate begin at:      " + startMs);

        // Populate the Redis Cache keys
        for (int i = 0; i < count; i++) {
            char c = chars.charAt(rnd.nextInt(chars.length()));
            String key = "test_" + c;
            //log.warn("incrementing key: " + key);
            syncCommands.incr(key);  // <-- increments the given key in the DB
        }

        long finishMs = System.currentTimeMillis();
        log.warn("populate finish at:     " + finishMs);
        log.warn("populate elapsed ms:    " + (finishMs - startMs));

        // List and report on the incremented counts in the Redis Cache
        List<String> keys = syncCommands.keys("test*");
        for (int k = 0; k < keys.size(); k++) {
            String key = keys.get(k);
            String value = syncCommands.get(key);
            log.warn("key: " + key + " -> " + value);
            // TODO - write the hourly totals to CosmosDB
        }
    }
}

//        Sample output; 50 with verbose logging:
//        14:40:38.032 [main] WARN  o.c.c.a.data.DataCommandLineApp - run method...
//        14:40:38.032 [main] WARN  o.c.c.a.data.DataAppConfiguration - setCommandLineArgs; length: 1
//        14:40:38.032 [main] WARN  o.c.c.a.data.DataAppConfiguration - setCommandLineArgs, idx: 0 -> test_redis
//        14:40:38.032 [main] WARN  o.c.c.a.data.DataCommandLineApp - run, processType: test_redis
//        14:40:38.213 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - RedisClient: io.lettuce.core.RedisClient@7d12429
//        14:40:38.220 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - initialCounter:        5108
//        14:40:38.220 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate loop count:    50
//        14:40:38.220 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate begin at:      1662144038220
//        14:40:38.220 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_b
//        14:40:38.221 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_x
//        14:40:38.221 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_x
//        14:40:38.222 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.223 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.224 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.225 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.225 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.226 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.226 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.227 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.228 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.228 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_z
//        14:40:38.229 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.229 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.230 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_z
//        14:40:38.230 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_b
//        14:40:38.231 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.231 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.232 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.232 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.232 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.233 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.233 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.234 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.234 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.235 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_z
//        14:40:38.235 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_x
//        14:40:38.236 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.236 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.237 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_z
//        14:40:38.237 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_z
//        14:40:38.238 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.238 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_b
//        14:40:38.238 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.239 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.240 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_x
//        14:40:38.240 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_b
//        14:40:38.241 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_y
//        14:40:38.241 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.242 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_c
//        14:40:38.242 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_z
//        14:40:38.243 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_x
//        14:40:38.243 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_b
//        14:40:38.244 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_x
//        14:40:38.244 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_b
//        14:40:38.245 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.245 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.245 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.246 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - incrementing key: test_a
//        14:40:38.246 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate finish at:     1662144038246
//        14:40:38.246 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate elapsed ms:    26
//        14:40:38.249 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_c -> 27
//        14:40:38.249 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_b -> 16
//        14:40:38.250 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_x -> 24
//        14:40:38.250 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_y -> 26
//        14:40:38.251 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test -> 5108
//        14:40:38.251 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_a -> 30
//        14:40:38.251 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_z -> 27
//        14:40:38.252 [main] WARN  o.c.c.a.data.DataCommandLineApp - spring app exiting

//        Another sample output; 5000 with less logging
//        14:43:30.291 [main] WARN  o.c.c.a.data.DataCommandLineApp - run method...
//        14:43:30.292 [main] WARN  o.c.c.a.data.DataAppConfiguration - setCommandLineArgs; length: 1
//        14:43:30.292 [main] WARN  o.c.c.a.data.DataAppConfiguration - setCommandLineArgs, idx: 0 -> test_redis
//        14:43:30.292 [main] WARN  o.c.c.a.data.DataCommandLineApp - run, processType: test_redis
//        14:43:30.490 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - RedisClient: io.lettuce.core.RedisClient@66e1b2a
//        14:43:30.496 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - initialCounter:        5109
//        14:43:30.496 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate loop count:    5000
//        14:43:30.496 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate begin at:      1662144210496
//        14:43:31.579 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate finish at:     1662144211579
//        14:43:31.580 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - populate elapsed ms:    1083
//        14:43:31.582 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_c -> 818
//        14:43:31.583 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_b -> 882
//        14:43:31.583 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_x -> 820
//        14:43:31.584 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_y -> 866
//        14:43:31.584 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test -> 5109
//        14:43:31.584 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_a -> 889
//        14:43:31.585 [main] WARN  o.c.c.a.d.p.RedisPocProcessor - key: test_z -> 875
//        14:43:31.585 [main] WARN  o.c.c.a.data.DataCommandLineApp - spring app exiting