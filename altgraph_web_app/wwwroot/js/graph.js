$(document).ready(function () {
    console.log("graph.html onready");

    function nodeClicked(e, d) {
        console.log("nodeClicked: " + d.name);
        var libName = d.name;
        $("#libraryInfo").text(libName + " ...");
        var httpRequestPromise = [
            fetch(`?handler=LibraryAsJson&libraryName=${libName}`)
        ];
        Promise.all(httpRequestPromise).then(function (responses) {
            const dataElement = document.querySelector("#graphData");

            dataElement.dataset.libraryAsJson.map(function (data) {
                console.log(data);
                var author = data.author.trim();
                var desc = data.desc.trim();
                var info = "Library: " + libName + "  ";
                if (author.length > 0) {
                    info = info + "author: " + author + "  ";
                }
                if (desc.length > 0) {
                    info = info + "  Description: " + desc;
                }
                $("#libraryInfo").text(info);
            });
        });
    }
    function nodeDblClicked(e, d) {
        console.log("nodeDblClicked: " + d.name);
        $("#libraryInfo").text("");
        $("#subjectName").val(d.name);
        d3.select("#viz").style("opacity", 0);  // hide the graph momentarily
        $("#searchForm").submit();
    }
    function nodeMouseOver(e, d) {
        //console.log("nodeMouseOver: " + d.name);
    }
    function nodeMouseOut(e, d) {
        //console.log("nodeMouseOut: " + d.name);
    }

    function initZoom() {
        d3.select('svg').call(zoom);
    }

    function handleZoom(e) {
        console.log('handle zoom');
        d3.select('svg g').attr('transform', e.transform);
    }

    function csvToArr(stringVal) {
        if (stringVal == null)
            return null;

        const [keys, ...rest] = stringVal
            .trim()
            .split("\r\n")
            .map((item) => item.split(","));

        const formedArr = rest.map((item) => {
            const object = {};
            keys.forEach((key, index) => (object[key] = item.at(index)));
            return object;
        });
        return formedArr;
    }

    let zoom = d3.zoom().on('zoom', handleZoom);

    var typeScale = d3.scaleOrdinal()
        .domain(["library", "author", "maintainer"])
        .range(["#75739F", "#41A368", "#FE9922"]);

    // See https://datawanderings.com/2018/08/15/d3-js-v5-promise-syntax-examples/
    var nodes = Array();
    var edges = Array();

    const dataElement = document.querySelector("#graphData");

    console.log("csv data should now be loaded");
    const nodesCsv = csvToArr(dataElement.dataset.nodesCsv == "" ? null : dataElement.dataset.nodesCsv);
    nodesCsv?.map(function (n) {
        console.log(n);
        nodes.push(n);
    });
    const edgesCsv = csvToArr(dataElement.dataset.edgesCsv == "" ? null : dataElement.dataset.edgesCsv);
    edgesCsv?.map(function (e) {
        console.log(e);
        edges.push(e);
    });
    console.log("csv data should now be loaded");
    console.log("nodes: " + nodes.length);
    console.log("edges: " + edges.length);
    generateGraphViz(nodes, edges);

    function generateGraphViz(nodes, edges) {
        console.log("generateGraphViz");

        var marker = d3.select("svg").append('defs')
            .append('marker')
            .attr("id", "Triangle")
            .attr("refX", 12)
            .attr("refY", 6)
            .attr("markerUnits", 'userSpaceOnUse')
            .attr("markerWidth", 12)
            .attr("markerHeight", 18)
            .attr("orient", 'auto')
            .append('path')
            .attr("d", 'M 0 0 12 6 0 12 3 6');

        var nodeHash = {};
        nodes.forEach(node => {
            node.weight = parseInt(node.weight);
            nodeHash[node.name] = node;
        });

        nodes.forEach(node => {
            console.log("node: " + node.name);
        });

        edges.forEach(edge => {
            edge.weight = parseInt(edge.weight);
            edge.source = nodeHash[edge.source];
            edge.target = nodeHash[edge.target];
        });

        var linkForce = d3.forceLink(edges);

        var simulation = d3.forceSimulation()
            .force("charge", d3.forceManyBody().strength(-2000))
            .force("center", d3.forceCenter().x(800).y(500))
            .force("link", linkForce)
            .nodes(nodes)
            .on("tick", forceTick);

        simulation.force("link").links(edges);

        d3.select("svg g").selectAll("line.link")
            .data(edges, d => `${d.source.id}-${d.target.id}`)
            .enter()
            .append("line")
            .attr("class", "link")
            .style("opacity", .5)
            .style("stroke-width", d => d.weight);

        d3.selectAll("line").attr("marker-end", "url(#Triangle)");

        var nodeEnter = d3.select("svg g").selectAll("g.node")
            .data(nodes, d => d.name)
            .enter()
            .append("g")
            .attr("class", "node");
        nodeEnter.append("circle")
            .attr("r", 5)
            .style("fill", d => typeScale(d.type));
        nodeEnter.append("text")
            .style("text-anchor", "middle")
            .attr("y", 15)
            .text(d => d.name);

        // Register mouse event handler functions for each Node
        d3.selectAll("g.node").on("click", function (e, d) {
            nodeClicked(e, d);
        });
        d3.selectAll("g.node").on("dblclick", function (e, d) {
            nodeDblClicked(e, d);
        });
        d3.selectAll("g.node").on("mouseover", function (e, d) {
            nodeMouseOver(e, d);
        });
        d3.selectAll("g.node").on("mouseout", function (e, d) {
            nodeMouseOut(e, d);
        });

        function forceTick() {
            d3.selectAll("line.link")
                .attr("x1", d => d.source.x)
                .attr("x2", d => d.target.x)
                .attr("y1", d => d.source.y)
                .attr("y2", d => d.target.y);
            d3.selectAll("g.node")
                .attr("transform", d => `translate(${d.x},${d.y})`);
        }

        initZoom();
    }
});