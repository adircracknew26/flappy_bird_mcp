"""Genera los sprites pixel art de Flappy Bird.

Uso:  python Tools/generate_art.py
Salida: Assets/FlappyBird/Resources/BirdFrames y Assets/FlappyBird/Sprites
"""
from pathlib import Path
import random

from PIL import Image, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
SPRITES = ROOT / "Assets" / "FlappyBird" / "Sprites"
BIRD = ROOT / "Assets" / "FlappyBird" / "Resources" / "BirdFrames"
SPRITES.mkdir(parents=True, exist_ok=True)
BIRD.mkdir(parents=True, exist_ok=True)

# ---------- Paleta ----------
OUTLINE = (90, 58, 20, 255)
BODY = (255, 217, 59, 255)
BELLY = (255, 236, 130, 255)
WING = (242, 183, 46, 255)
BEAK = (242, 140, 40, 255)
WHITE = (255, 255, 255, 255)
PUPIL = (43, 27, 14, 255)

PIPE_OUTLINE = (42, 94, 16, 255)
PIPE_BASE = (99, 185, 46, 255)
PIPE_LIGHT = (155, 224, 90, 255)
PIPE_LIGHT2 = (124, 205, 66, 255)
PIPE_DARK = (61, 138, 26, 255)

GRASS_DARK = (46, 94, 20, 255)
GRASS = (106, 190, 48, 255)
GRASS_LIGHT = (147, 224, 82, 255)
DIRT = (201, 138, 75, 255)
DIRT_DARK = (174, 113, 56, 255)
DIRT_LIGHT = (217, 162, 102, 255)
DIRT_EDGE = (138, 90, 46, 255)


def bird_frame(wing_y: int) -> Image.Image:
    """Pájaro de 32x32, se escala x2 para look pixel art."""
    img = Image.new("RGBA", (32, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    # Cola
    d.polygon([(5, 13), (1, 9), (1, 21), (5, 18)], fill=OUTLINE)
    d.polygon([(6, 14), (2, 11), (2, 19), (6, 17)], fill=BODY)

    # Cuerpo
    d.ellipse((3, 5, 27, 28), fill=OUTLINE)
    d.ellipse((4, 6, 26, 27), fill=BODY)
    d.ellipse((9, 16, 25, 27), fill=BELLY)

    # Ala (3 posiciones)
    d.ellipse((6, wing_y, 17, wing_y + 8), fill=OUTLINE)
    d.ellipse((7, wing_y + 1, 16, wing_y + 7), fill=WING)

    # Ojo
    d.ellipse((17, 8, 24, 16), fill=OUTLINE)
    d.ellipse((18, 9, 23, 15), fill=WHITE)
    d.ellipse((20, 10, 23, 14), fill=PUPIL)
    d.point((20, 10), fill=WHITE)

    # Pico
    d.polygon([(24, 17), (31, 20), (24, 23)], fill=OUTLINE)
    d.polygon([(24, 18), (30, 20), (24, 22)], fill=BEAK)

    # Brillo del cuerpo
    d.arc((6, 7, 20, 19), 200, 260, fill=(255, 255, 255, 120), width=1)

    return img.resize((64, 64), Image.Resampling.NEAREST)


def make_bird():
    for i, wing_y in enumerate((11, 14, 17)):
        bird_frame(wing_y).save(BIRD / f"bird_{i}.png")


def make_cloud():
    img = Image.new("RGBA", (96, 48), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    puffs = [(4, 24, 44, 45), (22, 12, 64, 45), (44, 20, 86, 45), (14, 25, 82, 46)]
    # Sombra inferior
    for box in puffs:
        d.ellipse((box[0], box[1] + 2, box[2], box[3] + 2), fill=(212, 236, 240, 220))
    for box in puffs:
        d.ellipse(box, fill=(255, 255, 255, 245))
    img.resize((192, 96), Image.Resampling.NEAREST).save(SPRITES / "cloud.png")


def make_pipe():
    body = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
    d = ImageDraw.Draw(body)
    d.rectangle((2, 0, 61, 63), fill=PIPE_OUTLINE)
    d.rectangle((4, 0, 59, 63), fill=PIPE_BASE)
    d.rectangle((8, 0, 16, 63), fill=PIPE_LIGHT)
    d.rectangle((19, 0, 21, 63), fill=PIPE_LIGHT2)
    d.rectangle((48, 0, 59, 63), fill=PIPE_DARK)
    # Textura sutil
    for y in range(0, 64, 16):
        d.rectangle((22, y + 3, 46, y + 4), fill=(90, 170, 40, 255))
        d.rectangle((22, y + 11, 40, y + 12), fill=(112, 196, 58, 255))
    body.save(SPRITES / "pipe_body.png")

    cap = Image.new("RGBA", (96, 32), (0, 0, 0, 0))
    d = ImageDraw.Draw(cap)
    d.rounded_rectangle((2, 1, 93, 30), radius=6, fill=PIPE_OUTLINE)
    d.rounded_rectangle((4, 3, 91, 28), radius=5, fill=PIPE_BASE)
    d.rectangle((7, 5, 88, 11), fill=PIPE_LIGHT)
    d.rectangle((7, 20, 88, 26), fill=PIPE_DARK)
    d.rectangle((7, 6, 12, 24), fill=PIPE_LIGHT2)
    cap.save(SPRITES / "pipe_cap.png")


def make_ground():
    rnd = random.Random(7)
    w, h = 1024, 192
    img = Image.new("RGBA", (w, h), DIRT)
    d = ImageDraw.Draw(img)

    # Tierra
    d.rectangle((0, 44, w, h - 1), fill=DIRT)
    for _ in range(900):
        x = rnd.randint(0, w - 1)
        y = rnd.randint(52, h - 12)
        c = rnd.choice([DIRT_DARK, DIRT_LIGHT, (190, 126, 64, 255)])
        r = rnd.choice([1, 1, 2, 3])
        d.rectangle((x, y, x + r, y + r), fill=c)
    for _ in range(45):
        x = rnd.randint(0, w - 8)
        y = rnd.randint(70, h - 24)
        d.ellipse((x, y, x + rnd.randint(4, 9), y + rnd.randint(3, 6)), fill=DIRT_DARK)
        d.ellipse((x + 1, y, x + 4, y + 3), fill=DIRT_LIGHT)

    # Borde superior de hierba
    d.rectangle((0, 0, w, 5), fill=GRASS_DARK)
    d.rectangle((0, 5, w, 40), fill=GRASS)
    d.rectangle((0, 34, w, 40), fill=(84, 164, 38, 255))

    # Matas de hierba
    for x in range(0, w, 16):
        hh = rnd.choice([2, 3, 3, 4, 5])
        d.rectangle((x + 2, 5 - hh, x + 5, 5), fill=GRASS_LIGHT)
        d.rectangle((x + 9, 5 - max(1, hh - 1), x + 12, 5), fill=(122, 208, 62, 255))

    # Brillo y borde inferior
    for x in range(0, w, 24):
        d.rectangle((x + 5, 8, x + 15, 10), fill=(170, 232, 100, 255))
    d.rectangle((0, h - 10, w, h - 1), fill=DIRT_EDGE)
    d.rectangle((0, h - 4, w, h - 1), fill=(110, 70, 34, 255))

    img.save(SPRITES / "ground.png")


def make_sky():
    w, h = 1440, 1080
    top = (69, 183, 196)
    bottom = (214, 243, 240)
    img = Image.new("RGB", (w, h))
    px = img.load()
    for y in range(h):
        t = y / (h - 1)
        # Curva suave para que el celeste se mantenga arriba
        t = t ** 1.25
        r = round(top[0] + (bottom[0] - top[0]) * t)
        g = round(top[1] + (bottom[1] - top[1]) * t)
        b = round(top[2] + (bottom[2] - top[2]) * t)
        for x in range(w):
            px[x, y] = (r, g, b)

    # Sol suave (glow con desenfoque gaussiano para evitar anillos)
    glow = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    gd = ImageDraw.Draw(glow)
    gd.ellipse((1030, 50, 1270, 290), fill=(255, 249, 214, 150))
    glow = glow.filter(ImageFilter.GaussianBlur(55))

    core = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    cd = ImageDraw.Draw(core)
    cd.ellipse((1115, 135, 1185, 205), fill=(255, 252, 230, 130))
    core = core.filter(ImageFilter.GaussianBlur(12))

    img = Image.alpha_composite(img.convert("RGBA"), glow)
    img = Image.alpha_composite(img, core)
    img.convert("RGB").save(SPRITES / "sky.png")


if __name__ == "__main__":
    make_bird()
    make_cloud()
    make_pipe()
    make_ground()
    make_sky()
    print("Sprites generados en", SPRITES)
