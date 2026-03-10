from sentinelcareer_pythonworkers.adapters import parse_generic_jobs_html


def test_parse_generic_jobs_html_single():
    html = '<div class="job"><div class="title">VP Risk</div><div class="location">Mumbai</div></div>'
    result = parse_generic_jobs_html(html)
    assert result[0]["title"] == "VP Risk"


def test_parse_generic_jobs_html_multiple_and_missing_location():
    html = (
        '<div class="job"><div class="title">Director Governance</div><div class="location">Delhi</div></div>'
        '<div class="job"><div class="title">Head Compliance</div></div>'
    )
    result = parse_generic_jobs_html(html)
    assert len(result) == 2
    assert result[1]["location"] == ""
