package org.cjoakim.cosmos.altgraph.web.controller;

import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;

import javax.servlet.http.HttpSession;

/**
 * This controller is used to serve several static pages for this app,
 * implemented as Thymeleaf templates.
 * Chris Joakim, Microsoft, November 2022
 */

@Slf4j
@Controller
public class PagesController {

    @GetMapping(value = "/")
    public String home(HttpSession session, Model model) {
        return staticPage(session, model, "home");
    }

    @GetMapping(value = "/design")
    public String design(HttpSession session, Model model) {
        return staticPage(session, model, "design");
    }

    @GetMapping(value = "/architecture")
    public String architecture(HttpSession session, Model model) {
        return staticPage(session, model, "architecture");
    }

    @GetMapping(value = "/spring_data")
    public String springData(HttpSession session, Model model) {
        return staticPage(session, model, "spring_data");
    }

    private String staticPage(HttpSession session, Model model, String template) {
        String sessionId = session.getId();
        log.warn(template + ", session Id: " + sessionId);
        model.addAttribute("sessionId", sessionId);
        return template;
    }

}
