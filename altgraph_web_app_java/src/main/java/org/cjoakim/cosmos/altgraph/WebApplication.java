package org.cjoakim.cosmos.altgraph;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

/**
 * This is the entry-point to this Spring Web Application.
 *
 * Chris Joakim, Microsoft, November 2022
 */

@SpringBootApplication
//@ComponentScan(basePackages = { "org.cjoakim.cosmos.altgraph" })
public class WebApplication {

    public static void main(String[] args) {

        SpringApplication.run(WebApplication.class, args);
    }
}
