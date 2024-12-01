import { Book } from "@/lib/types";
import { useState } from "react";
import { supabase } from "@/utils/supabase/client";
import { url } from "inspector";

interface BookFormProps {
  CloseModal: () => void;
}

const BookForm = ({ CloseModal }: BookFormProps) => {
  const [title, setTitle] = useState("");
  const [author, setAuthor] = useState("");
  const [publicationDate, setPublicationDate] = useState("");
  const [totalPages, setTotalPages] = useState(0);
  const [description, setDescription] = useState("");
  const [cover, setCover] = useState<File | null>(null);

  const sanitizeFileName = (fileName: string) => {
    return fileName.replace(/[^a-zA-Z0-9.-]/g, "_");
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      let coverUrl = null;
      if (cover) {
        const file = cover;
        const sanitizedFileName = sanitizeFileName(`${title}-${file.name}`);
        const { data, error } = await supabase.storage
          .from("bookCover")
          .upload(`covers/${sanitizedFileName}`, file);

        if (error) {
          throw error;
        } else {
          console.log("Cover uploaded:", data);
        }

        const {
          data: { publicUrl },
        } = supabase.storage
          .from("bookCover")
          .getPublicUrl(`covers/${sanitizedFileName}`);

        coverUrl = publicUrl;
      }

      const newBook = {
        title,
        author,
        publication_date: publicationDate,
        total_pages: totalPages,
        description,
        cover_url: coverUrl,
      };

      const { data, error } = await supabase.from("books").insert([newBook]);

      if (error) {
        throw error;
      }

      console.log("Book added:", data);
      CloseModal();
    } catch (error) {
      console.error("Error adding book:", error);
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="h-auto w-[600px] bg-white rounded-lg p-5 "
    >
      <div className="flex justify-between  mb-4">
        <h1 className="text-2xl font-bold  ">Book details</h1>
        <button
          className="hover:bg-red-500 px-2 font-bold rounded-lg"
          onClick={CloseModal}
        >
          X
        </button>
      </div>
      <div className=" flex flex-col gap-5">
        <div className="flex flex-col  rounded-lg gap-2 ">
          <label className=" font-bold" htmlFor="title">
            Title
          </label>
          <input
            onChange={(e) => setTitle(e.target.value)}
            className="border p-2 rounded-lg"
            type="text"
            name="title"
            placeholder="Title of the book"
          />
        </div>
        <div className="flex flex-col  rounded-lg gap-2 ">
          <label className=" font-bold" htmlFor="Aothor">
            Author
          </label>
          <input
            onChange={(e) => setAuthor(e.target.value)}
            className="border p-2 rounded-lg"
            type="text"
            name="Athor"
            placeholder="Authors full name"
          />
        </div>
        <div className="flex flex-col  rounded-lg gap-2 ">
          <label className=" font-bold" htmlFor="publication date ">
            Publication date
          </label>
          <input
            onChange={(e) => setPublicationDate(e.target.value)}
            className="border p-2 rounded-lg"
            type="number"
            name="publication date"
            placeholder="Publication date of the book"
          />
        </div>
        <div className="flex flex-col  rounded-lg gap-2 ">
          <label className=" font-bold" htmlFor="pages ">
            Total pages
          </label>
          <input
            onChange={(e) => setTotalPages(parseInt(e.target.value))}
            className="border p-2 rounded-lg"
            type="number"
            name="publication date"
            placeholder="Total pages of the book"
          />
        </div>
        <div className="flex flex-col  rounded-lg gap-2 ">
          <label className=" font-bold" htmlFor="pages ">
            Book cover
          </label>
          <input
            onChange={(e) => {
              if (e.target.files) {
                setCover(e.target.files[0]);
              }
            }}
            className="border p-2 rounded-lg"
            type="file"
          />
        </div>
        <div className="flex flex-col rounded-lg gap-2 ">
          <label className=" font-bold" htmlFor="title">
            Description
          </label>
          <textarea
            onChange={(e) => setDescription(e.target.value)}
            className="border p-2 rounded-lg h-32"
            name="description"
            id="description"
            placeholder="Short Description of the book"
          />
        </div>
      </div>
      <button
        className="border rounded-xl p-2 mt-2 bg-green-500 font-bold hover:scale-x-110"
        type="submit"
        onClick={handleSubmit}
      >
        Add book
      </button>
    </form>
  );
};
export default BookForm;
