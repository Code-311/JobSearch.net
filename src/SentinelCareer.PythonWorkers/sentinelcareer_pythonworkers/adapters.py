import re


def _extract(css_class: str, chunk: str) -> str:
    m = re.search(rf'class="{css_class}"\s*>\s*([^<]+)', chunk, flags=re.IGNORECASE)
    return m.group(1).strip() if m else ""


def parse_generic_jobs_html(html: str) -> list[dict]:
    jobs = []
    for chunk in re.findall(r'<div\s+class="job"[^>]*>(.*?)</div>', html, flags=re.IGNORECASE | re.DOTALL):
        title = _extract("title", chunk)
        location = _extract("location", chunk)
        if title:
            jobs.append({"title": title, "location": location})
    if not jobs and 'class="job"' in html.lower():
        title = _extract("title", html)
        if title:
            jobs.append({"title": title, "location": _extract("location", html)})
    return jobs
