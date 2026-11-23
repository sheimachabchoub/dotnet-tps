from pathlib import Path
import re
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.section import WD_SECTION
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_CELL_VERTICAL_ALIGNMENT
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

ROOT = Path(r"D:\.net\repos\TP6_WEBAPI")
SOURCE = ROOT / "CompteRendu_TP6.md"
OUTPUT = ROOT / "CompteRendu_TP6.docx"

def set_cell_shading(cell, fill):
    tc_pr = cell._tc.get_or_add_tcPr()
    shd = tc_pr.find(qn("w:shd"))
    if shd is None:
        shd = OxmlElement("w:shd")
        tc_pr.append(shd)
    shd.set(qn("w:fill"), fill)

def set_cell_borders(cell, color="D9D9D9"):
    tc = cell._tc
    tc_pr = tc.get_or_add_tcPr()
    borders = tc_pr.first_child_found_in("w:tcBorders")
    if borders is None:
        borders = OxmlElement("w:tcBorders")
        tc_pr.append(borders)
    for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
        tag = "w:" + edge
        element = borders.find(qn(tag))
        if element is None:
            element = OxmlElement(tag)
            borders.append(element)
        element.set(qn("w:val"), "single")
        element.set(qn("w:sz"), "4")
        element.set(qn("w:space"), "0")
        element.set(qn("w:color"), color)

def set_font(run, name="Aptos", size=10.5, bold=False, color="000000"):
    run.font.name = name
    run._element.get_or_add_rPr().rFonts.set(qn("w:ascii"), name)
    run._element.get_or_add_rPr().rFonts.set(qn("w:hAnsi"), name)
    run.font.size = Pt(size)
    run.bold = bold
    run.font.color.rgb = RGBColor.from_string(color)

def add_code(doc, text):
    p = doc.add_paragraph(style="Code Block")
    p.paragraph_format.left_indent = Inches(0.18)
    p.paragraph_format.right_indent = Inches(0.18)
    for line in text.rstrip("\n").splitlines() or [""]:
        r = p.add_run(line + "\n")
        set_font(r, "Consolas", 8.5, color="1F2937")
    return p

def add_inline(p, text):
    parts = re.split(r"(`[^`]+`|\*\*[^*]+\*\*)", text)
    for part in parts:
        if not part:
            continue
        if part.startswith("**") and part.endswith("**"):
            r = p.add_run(part[2:-2]); set_font(r, bold=True)
        elif part.startswith("`") and part.endswith("`"):
            r = p.add_run(part[1:-1]); set_font(r, "Consolas", 9, color="17365D")
        else:
            r = p.add_run(part); set_font(r)

def add_placeholder(doc, text):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r = p.add_run(text)
    set_font(r, "Aptos", 10, bold=True, color="666666")
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)

def add_image(doc, path, caption):
    if path.exists():
        p = doc.add_paragraph()
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.add_run().add_picture(str(path), width=Inches(6.2))
        c = doc.add_paragraph()
        c.alignment = WD_ALIGN_PARAGRAPH.CENTER
        r = c.add_run(caption); set_font(r, size=9, color="666666")
    else:
        add_placeholder(doc, f"[ Capture à insérer : {caption} ]")

def add_table(doc, rows):
    table = doc.add_table(rows=1, cols=len(rows[0]))
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.style = "Table Grid"
    for i, value in enumerate(rows[0]):
        cell = table.rows[0].cells[i]
        cell.text = str(value)
        set_cell_shading(cell, "1F4E78")
        cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
        set_cell_borders(cell)
        for r in cell.paragraphs[0].runs:
            set_font(r, size=9, bold=True, color="FFFFFF")
    for ridx, row in enumerate(rows[1:]):
        cells = table.add_row().cells
        for i, value in enumerate(row):
            cells[i].text = str(value)
            cells[i].vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
            if ridx % 2 == 1:
                set_cell_shading(cells[i], "F2F6FA")
            set_cell_borders(cells[i])
            for r in cells[i].paragraphs[0].runs:
                set_font(r, size=8.5)
    doc.add_paragraph().paragraph_format.space_after = Pt(2)

def build():
    doc = Document()
    sec = doc.sections[0]
    sec.top_margin = Inches(0.7); sec.bottom_margin = Inches(0.7)
    sec.left_margin = Inches(0.8); sec.right_margin = Inches(0.8)

    styles = doc.styles
    normal = styles["Normal"]
    normal.font.name = "Aptos"; normal._element.rPr.rFonts.set(qn("w:ascii"), "Aptos"); normal._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos")
    normal.font.size = Pt(10.5)
    normal.paragraph_format.space_after = Pt(5)
    for name, size, color in [("Title", 24, "000000"), ("Heading 1", 16, "000000"), ("Heading 2", 12.5, "000000"), ("Heading 3", 11, "000000")]:
        st = styles[name]; st.font.name = "Aptos Display"; st._element.rPr.rFonts.set(qn("w:ascii"), "Aptos Display"); st._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos Display"); st.font.size = Pt(size); st.font.color.rgb = RGBColor.from_string(color); st.font.bold = True
    if "Code Block" not in [s.name for s in styles]:
        cs = styles.add_style("Code Block", 1)
        cs.font.name = "Consolas"; cs.font.size = Pt(8.5); cs.paragraph_format.space_before = Pt(3); cs.paragraph_format.space_after = Pt(6)

    # Cover page
    p = doc.add_paragraph(style="Title"); p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r = p.add_run("Compte rendu TP 6"); set_font(r, "Aptos Display", 26, bold=True)
    p2 = doc.add_paragraph(); p2.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r = p2.add_run("Web API de gestion des comptes bancaires"); set_font(r, size=15, color="1F4E78")
    doc.add_paragraph()
    for label, value in [("Projet", "TP6_WEBAPI"), ("Technologie", "ASP.NET Core Web API .NET 8"), ("Base de données", "SQL Server LocalDB"), ("Documentation", "Tests réalisés avec Swagger UI")]:
        p = doc.add_paragraph(); p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        r = p.add_run(f"{label} : "); set_font(r, bold=True)
        r = p.add_run(value); set_font(r)
    doc.add_page_break()

    lines = SOURCE.read_text(encoding="utf-8").splitlines()
    i = 0
    while i < len(lines):
        line = lines[i]
        if not line.strip() or line.startswith(">"):
            if line.startswith(">"):
                p = doc.add_paragraph(); add_inline(p, line[1:].strip())
            i += 1; continue
        if line.startswith("# "):
            # Skip markdown title already represented on cover.
            i += 1; continue
        if line.startswith("## "):
            p = doc.add_paragraph(line[3:].strip(), style="Heading 1"); i += 1; continue
        if line.startswith("### "):
            p = doc.add_paragraph(line[4:].strip(), style="Heading 2"); i += 1; continue
        if line.startswith("#### "):
            p = doc.add_paragraph(line[5:].strip(), style="Heading 3"); i += 1; continue
        if line.startswith("```"):
            code = []; i += 1
            while i < len(lines) and not lines[i].startswith("```"):
                code.append(lines[i]); i += 1
            i += 1; add_code(doc, "\n".join(code)); continue
        if line.startswith("|"):
            rows = []
            while i < len(lines) and lines[i].startswith("|"):
                vals = [x.strip() for x in lines[i].strip().strip("|").split("|")]
                if not all(set(x) <= set("-: ") for x in vals): rows.append(vals)
                i += 1
            if rows: add_table(doc, rows)
            continue
        if re.match(r"^[-*] \[[ x]\]", line):
            checked = "☑" if "[x]" in line else "☐"
            text = re.sub(r"^[-*] \[[ x]\]", "", line).strip()
            p = doc.add_paragraph(style="List Bullet"); add_inline(p, f"{checked} {text}"); i += 1; continue
        if line.startswith("- ") or re.match(r"^\d+\. ", line):
            text = re.sub(r"^(- |\d+\. )", "", line)
            p = doc.add_paragraph(style="List Bullet" if line.startswith("-") else "List Number"); add_inline(p, text); i += 1; continue
        image_match = re.match(r"!\[([^]]*)\]\(([^)]+)\)", line)
        if image_match:
            caption, rel = image_match.groups(); add_image(doc, ROOT / rel.replace("/", "\\"), caption); i += 1; continue
        if line.startswith("**Emplacement :**"):
            p = doc.add_paragraph(); add_inline(p, line); i += 1; continue
        if line.startswith("[ INSÉRER"):
            add_placeholder(doc, line); i += 1; continue
        p = doc.add_paragraph(); add_inline(p, line); i += 1

    # Footer
    for section in doc.sections:
        fp = section.footer.paragraphs[0]; fp.alignment = WD_ALIGN_PARAGRAPH.CENTER
        r = fp.add_run("TP6_WEBAPI - Compte rendu"); set_font(r, size=8, color="666666")
    doc.core_properties.title = "Compte rendu TP 6 Web API"
    doc.core_properties.subject = "Gestion des comptes bancaires avec ASP.NET Core Web API"
    doc.core_properties.author = "Étudiant"
    doc.save(OUTPUT)

if __name__ == "__main__":
    build()
